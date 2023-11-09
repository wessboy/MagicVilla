using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dtos;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MagicVilla_Web.Controllers
{
    public class AuthController : Controller
    {

        private readonly IAuthService _auth;
        
        public AuthController(IAuthService authService)
        {
            _auth = authService;
            ;

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<UserDTO> users = new List<UserDTO>();

            var response = await _auth.GetUsersAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {

                users = JsonConvert.DeserializeObject<List<UserDTO>>(Convert.ToString(response.Result));
            }

            return View(users);

        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO obj = new();
            return View(obj);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO obj)
        {
            APIResponse response = await _auth.LoginAsync<APIResponse>(obj);
            if(response != null  && response.IsSuccess)
            {
                LoginResponseDTO model = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));

                //retrive Claims : role & userName from Token
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(model.Token);

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name,jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
                identity.AddClaim(new Claim(ClaimTypes.Role,jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
                
                HttpContext.Session.SetString(SD.SessionToken,model.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError",response.ErrorMessages.FirstOrDefault());
                return View(obj);
            }
            
           
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roles = new List<SelectListItem>()
            {
                new SelectListItem() {Text = SD.Admin , Value = SD.Admin},
                new SelectListItem(){Text = SD.Customer, Value = SD.Customer},
            };
            ViewBag.RolesList = roles;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationRequestDTO obj)
        {
            var result = await _auth.RegisterAsync<APIResponse>(obj);

            if(result != null && result.IsSuccess)
            {
                return RedirectToAction("Login");
            }

            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(SD.SessionToken,"");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


      /*  [HttpGet]
        public async Task<IActionResult> UpdateUser(string id)
        {
            var response = await _auth.GetUser<APIResponse>(id);
            if (response != null && response.IsSuccess)
            {

              UserDTO  userDto = JsonConvert.DeserializeObject<UserDTO>(Convert.ToString(response.Result));

                UpdatingRequestDTO updatingRequestDTO = new()
                {
                    UserUpdateDTO = new()
                    {
                        Id = userDto.Id,
                        Name = userDto.Name,
                        UserName = userDto.UserName,
                    },
                    Role = ""
                };
                return View(updatingRequestDTO);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(string id,UpdatingRequestDTO updatingRequestDTO)
        {
            if(ModelState.IsValid)
            {
                var response = await _auth.UpdateUserAsync<APIResponse>(id,updatingRequestDTO);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa updated successfully.";
                    return RedirectToAction("Index","Auth");
                }
               
            }

            TempData["error"] = "Error encountred.";
            return View(updatingRequestDTO);
        }*/


    }
}
