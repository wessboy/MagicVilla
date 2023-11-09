
using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dtos;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace MagicVilla_VillaAPI.Controllers.V2
{
    [Route("api/v{version:ApiVersion}/VillaAPI")]
    [ApiController]
    [ApiVersion("2.0")]
    public class VillaAPIController : ControllerBase
    {

        protected APIResponse _response;
        private readonly IVillaRepository _dbVilla;
        private IMapper _mapper;
        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            _mapper = mapper;
            _dbVilla = dbVilla;
            _response = new();
        }



        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Villa>>> GetVillas()
        {
            try
            {
                List<Villa> villas = await _dbVilla.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaDTO>>(villas);
                _response.Status = HttpStatusCode.OK;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }


            return Ok(_response);
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "user,admin")]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {

                    return BadRequest();
                }

                Villa villa = await _dbVilla.GetAsync(u => u.Id == id);

                if (villa == null)
                    return NotFound();


                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.Status = HttpStatusCode.OK;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return Ok(_response);

        }




        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            try
            {
                if (await _dbVilla.GetAsync(u => u.Name == createDTO.Name) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa already Exists !");
                    return BadRequest(ModelState);
                }
                if (createDTO == null)
                    return BadRequest();



                Villa villa = _mapper.Map<Villa>(createDTO);
                await _dbVilla.CreateAsync(villa);

                _response.Result = villa;
                _response.Status = HttpStatusCode.Created;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }



            return _response;
        }




        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest();

                Villa villa = await _dbVilla.GetAsync(v => v.Id == id);

                if (villa == null)
                    return NotFound();

                await _dbVilla.RemoveAsync(villa);
                _response.Status = HttpStatusCode.NoContent;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;

        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id == 0 || id != updateDTO.Id)
                    return BadRequest();

                Villa villaFromDb = await _dbVilla.GetAsync(v => v.Id == id, tracked: false);
                if (villaFromDb == null)
                    return NotFound();

                Villa villa = _mapper.Map<Villa>(updateDTO);



                await _dbVilla.UpdateAsync(villa);

                _response.IsSuccess = true;
                _response.Status = HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };

            }


            return _response;

        }




        [HttpPatch("{id:int}", Name = "UpadatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id, [FromBody] JsonPatchDocument<VillaUpdateDTO> patchDTo)
        {
            try
            {
                if (id == 0 || patchDTo == null)
                {
                    return BadRequest();
                }

                Villa villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);

                if (villa == null)
                {
                    return NotFound();
                }
                VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

                patchDTo.ApplyTo(villaDTO, ModelState);
                Villa villaModel = _mapper.Map<Villa>(villaDTO);

                await _dbVilla.UpdateAsync(villaModel);

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                _response.IsSuccess = true;
                _response.Status = HttpStatusCode.NoContent;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }


            return _response;

        }

    }
}
