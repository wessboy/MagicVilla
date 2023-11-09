using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dtos;

namespace MagicVilla_Web.Services.IServices
{
    public interface IAuthService 
    {
        Task<T> LoginAsync<T>(LoginRequestDTO obj);

        Task<T> RegisterAsync<T>(RegistrationRequestDTO obj);

        Task<T> UpdateUserAsync<T>(string id,UpdatingRequestDTO obj);

        Task<T> GetUsersAsync<T>();

        Task<T> GetUser<T>(string id);
    }
}
