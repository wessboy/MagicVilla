using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dtos;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        public UsersController(IUserRepository userRepository)
        {
            _userRpo = userRepository;
            this._response = new APIResponse();
        }

        // Action Methods

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
