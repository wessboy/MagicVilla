using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dtos;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/UsersAuth")]
    [ApiController]
    [ApiVersionNeutral]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRpo;
        private APIResponse _response;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository userRepository,IMapper mapper)
        {
            _userRpo = userRepository;
            this._response = new APIResponse();
            _mapper = mapper;
        }

        [HttpGet("AllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                 
                List<UserDTO> usersDTOList = await _userRpo.GetUsers(); 
                _response.Result = usersDTOList;
                _response.Status = HttpStatusCode.OK;

            }
            catch (Exception e)
            {

                _response.IsSuccess = false;
                _response.Status = HttpStatusCode.NotFound;
                _response.ErrorMessages = new List<string> {e.ToString() };
            }

            return Ok(_response);
        }

        [HttpGet("{id}",Name ="GetUser")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _response.Status = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                UserDTO userDTO = await _userRpo.GetUser(id);

                if(userDTO == null)
                {
                    _response.IsSuccess = false;
                    _response.Status = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = userDTO;
                _response.Status = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Status = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>()
                {
                    e.ToString()
                };
             
            }

            return Ok(_response) ;

        }

        // Action Methods
        [HttpPut("{id}",Name ="EditUser")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdatingRequestDTO updatingRequestDTO)
        {
            try
            {
                if(updatingRequestDTO == null || string.IsNullOrEmpty(id))
                {
                    _response.IsSuccess=false;
                    _response.Status = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                UserDTO userDto = await _userRpo.UpdateUser(id,updatingRequestDTO);
                if(userDto == null)
                {
                    _response.IsSuccess=false;
                    _response.Status=HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Inalid User Credentials !");
                    return BadRequest(_response);
                    
                }

                _response.Result=userDto;
                _response.Status = HttpStatusCode.OK;


            }
            catch (Exception e)
            {

                _response.IsSuccess=false;
                _response.Status=HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { e.ToString() };
            }
            return Ok(_response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO) {
            
           LoginResponseDTO loginResponse = await _userRpo.Login(loginRequestDTO);

            if(loginResponse.User == null || loginResponse.Token.IsNullOrEmpty())
            {
                _response.IsSuccess = false;
                _response.Status = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Invalid userName or Password !");

                return BadRequest(_response);
            }

            _response.Status = HttpStatusCode.OK;
            _response.Result = loginResponse;

            return Ok(_response);

        }



        /*        REGISTER ACTION         */

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequestDTO )
        {
            bool isUnique =  _userRpo.IsUniqueUser(registrationRequestDTO.UserName);

            if(!isUnique)
            {
                _response.IsSuccess=false;
                _response.Status=HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("UserName Already Exists !");

                return BadRequest(_response);

            }

            var user = await _userRpo.Register(registrationRequestDTO);

            if(user == null) { 
            
                 _response.IsSuccess = false;
                _response.Status=HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Error While registaring");
            }

            _response.Status = HttpStatusCode.OK;
            _response.Result = user;

            return Ok(_response);

        }


        /* CONTROLLER END */
    }
}
