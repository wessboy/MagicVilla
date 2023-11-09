using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dtos;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography.Xml;

namespace MagicVilla_VillaAPI.Controllers.V2
{
    [Route("api/v{version:ApiVersion}/VillaNumberAPI")]
    [ApiController]
    [ApiVersion("2.0")]
    public class VillaNumberAPIController : ControllerBase
    {
        private readonly IVillaNubmerRepository _dbVillaNumber;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public VillaNumberAPIController(IVillaNubmerRepository dbVillaNumber, IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVillaNumber = dbVillaNumber;
            _dbVilla = dbVilla;
            _mapper = mapper;
            _response = new();

        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(Duration = 30)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                List<VillaNumber> villaNumbers = await _dbVillaNumber.GetAllAsync(includeProperties: "Villa");
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbers);
                _response.Status = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpGet("{villaNo:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNo)
        {
            try
            {
                if (villaNo == 0)
                {
                    return BadRequest();
                }

                VillaNumber villaNumber = await _dbVillaNumber.GetAsync(n => n.VillaNo == villaNo, includeProperties: "Villa");
                if (villaNumber == null)
                {
                    return NotFound();
                }

                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.Status = HttpStatusCode.OK;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreatedDTO createdDTO)
        {
            try
            {
                if (createdDTO == null)
                {
                    return BadRequest();
                }
                if (await _dbVillaNumber.GetAsync(v => v.VillaNo == createdDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Number Already Exist !");
                    return BadRequest(ModelState);
                }
                if (await _dbVilla.GetAsync(v => v.Id == createdDTO.VillaId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Doesn't Exist !");

                    return BadRequest(ModelState);
                }
                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createdDTO);
                await _dbVillaNumber.CreateAsync(villaNumber);

                _response.Result = villaNumber;
                _response.Status = HttpStatusCode.Created;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }


        [HttpDelete("{villaNo:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int villaNo)
        {
            try
            {
                if (villaNo == 0)
                {
                    return BadRequest();
                }

                VillaNumber villaNumber = await _dbVillaNumber.GetAsync(n => n.VillaNo == villaNo);
                if (villaNumber == null)
                {
                    return NotFound();
                }

                await _dbVillaNumber.RemoveAsync(villaNumber);
                _response.Status = HttpStatusCode.NoContent;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpPut("{villaNo:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo, [FromBody] VillaNumberUpdateDTO updateDTO)
        {
            try
            {
                if (villaNo == 0 || updateDTO == null || villaNo != updateDTO.VillaNo)
                {
                    ModelState.AddModelError("ErrorMessages", "Invalid Input !");
                    return BadRequest();
                }
                if (await _dbVilla.GetAsync(v => v.Id == updateDTO.VillaId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Doesn't Exist !");

                    return BadRequest(ModelState);
                }

                VillaNumber villaNumber = await _dbVillaNumber.GetAsync(n => n.VillaNo == villaNo, tracked: false);

                if (villaNumber == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Number Doesn't Exist !");
                    return NotFound();
                }

                VillaNumber VillaNumberModel = _mapper.Map<VillaNumber>(updateDTO);

                await _dbVillaNumber.UpdateAsync(VillaNumberModel);
                _response.Status = HttpStatusCode.NoContent;


            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpPatch("{villaNo:int}", Name = "UpdatePartialVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVillaNumber(int villaNo, [FromBody] JsonPatchDocument<VillaNumberUpdateDTO> patchDTO)
        {
            try
            {
                if (villaNo == 0 || patchDTO == null)
                {
                    return BadRequest();
                }

                VillaNumber villaNumberDb = await _dbVillaNumber.GetAsync(v => v.VillaNo == villaNo, tracked: false);

                if (villaNumberDb == null) { return NotFound(); }

                VillaNumberUpdateDTO updateDTO = _mapper.Map<VillaNumberUpdateDTO>(villaNumberDb);

                if (await _dbVilla.GetAsync(v => v.Id == updateDTO.VillaId) == null)
                {
                    ModelState.AddModelError("Custom Error", "Villa Doesn't Exist !");

                    return BadRequest(ModelState);
                }

                patchDTO.ApplyTo(updateDTO, ModelState);

                VillaNumber villaNumberModel = _mapper.Map<VillaNumber>(updateDTO);
                await _dbVillaNumber.UpdateAsync(villaNumberModel);
                if (!ModelState.IsValid) { return BadRequest(); }

                _response.Status = HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };

            }

            return _response;
        }





    }
}
