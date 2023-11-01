﻿using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dtos;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
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

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name,model.User.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Role,model.User.Role));
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


    }
}