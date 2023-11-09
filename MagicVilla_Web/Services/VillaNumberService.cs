using MagicVilla_Web.Models.Dtos;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Microsoft.Extensions.Configuration;
using MagicVilla_Utility;

namespace MagicVilla_Web.Services
{
    public class VillaNumberService : BaseService , IVillaNubmerService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string villaNumberUrl;

        public VillaNumberService(IHttpClientFactory clientFactory,IConfiguration configuration):base(clientFactory)
        {
            _clientFactory = clientFactory;
            villaNumberUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
            
        }

        public Task<T> CreateAsync<T>(VillaNumberCreatedDTO dto, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Url = villaNumberUrl + $"/api/{SD.CurrentApiVersion}/VillaNumberAPI",
                Data = dto,
                Token = token
            });
        }

        public Task<T> DeleteAsync<T>(int id , string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = villaNumberUrl + $"/api/{SD.CurrentApiVersion}/VillaNumberAPI/" + id,
                Token = token
            });
        }

        public Task<T> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaNumberUrl + $"/api/{SD.CurrentApiVersion}/VillaNumberAPI",
                Token = token
            });
        }

        public Task<T> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaNumberUrl + $"/api/{SD.CurrentApiVersion}/VillaNumberAPI/" + id,
                Token = token
            });
        }

        public Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dto, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Url = villaNumberUrl + $"/api/{SD.CurrentApiVersion}/VillaNumberAPI/" +  dto.VillaNo,
                Data = dto,
                Token = token
            });
        }
    }
}
