using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private string secretKey;
        public UserRepository(ApplicationDbContext db,IMapper mapper,IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        

        public  bool IsUniqueUser(string username)
        {
            var user = _db.LocalUsers.FirstOrDefault(u => u.UserName == username);
                if(user == null)
            {
                return true;
            }
            return false;
        }

        

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            LoginResponseDTO loginResponseDTO;
            var user = await _db.LocalUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower() && u.Password == loginRequestDTO.Password);
            if(user == null)
            {
                  loginResponseDTO = new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };

                return loginResponseDTO;
            }

            //if user was found generate JWT Token

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),

                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
             loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = user
            };

            return loginResponseDTO;

        }

        public async Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            registrationRequestDTO.Role = "user";
            LocalUser user = _mapper.Map<LocalUser>(registrationRequestDTO);

            _db.LocalUsers.Add(user);
            await _db.SaveChangesAsync();
            user.Password = "";
            return user;
        }

        
    }
}
