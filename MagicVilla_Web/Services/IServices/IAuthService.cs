using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dtos;

namespace MagicVilla_Web.Services.IServices
{
    public interface IAuthService 
    {
        Task<T> LoginAsync<T>(LoginRequestDTO obj);

        Task<T> RegisterAsync<T>(RegistrationRequestDTO obj);
    }
}
