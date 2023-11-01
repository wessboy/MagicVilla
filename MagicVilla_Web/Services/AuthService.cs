using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dtos;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class AuthService : BaseService , IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string villaUrl;
        public AuthService(IHttpClientFactory httpClientFactory,IConfiguration configuration):base(httpClientFactory)
        {

            _httpClientFactory = httpClientFactory;

            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> LoginAsync<T>(LoginRequestDTO obj)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = villaUrl + "/api/UsersAuth/Login",

            });
        }

        public Task<T> RegisterAsync<T>(RegistrationRequestDTO obj)
        {
            return SendAsync<T>(new APIRequest { 
            
               ApiType = SD.ApiType.POST,
               Data = obj,
               Url = villaUrl + "/api/UsersAuth/Register"
            });
        }
    }
}
