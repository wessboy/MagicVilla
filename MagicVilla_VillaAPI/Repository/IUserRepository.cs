using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dtos;

namespace MagicVilla_VillaAPI.Repository
{
    public interface IUserRepository 
    {
      bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO);

        Task<UserDTO> UpdateUser(string id, UpdatingRequestDTO updatingRequestDTO);

        Task<List<UserDTO>> GetUsers();
        Task<UserDTO> GetUser(string id);
    }
}
