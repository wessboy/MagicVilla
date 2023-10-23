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

        public Task<T> CreateAsync<T>(VillaNumberCreatedDTO dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Url = villaNumberUrl + "/api/VillaNumberAPI",
                Data = dto
            });
        }

        public Task<T> DeleteAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = villaNumberUrl + "/api/VillaNumberAPI/" + id,
            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaNumberUrl + "/api/VillaNumberAPI",
            });
        }

        public Task<T> GetAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaNumberUrl + "/api/VillaNumberAPI/" + id,
            });
        }

        public Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Url = villaNumberUrl + "/api/VillaNumberAPI/" +  dto.VillaId,
                Data = dto
            });
        }
    }
}
