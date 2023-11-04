using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dtos;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey;
        public UserRepository(ApplicationDbContext db,IMapper mapper,IConfiguration configuration,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        

        public  bool IsUniqueUser(string username)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == username);
                if(user == null)
            {
                return true;
            }
            return false;
        }

        

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            LoginResponseDTO loginResponseDTO;
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());
             
            bool valid = await  _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            
            if(user == null || !valid)
            {
                  loginResponseDTO = new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };

                return loginResponseDTO;
            }

            //if user was found generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),

                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
             loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
                
            };

            return loginResponseDTO;

        }

        public async Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {


            ApplicationUser user = new() {
               UserName = registrationRequestDTO.UserName,
               Email = registrationRequestDTO.UserName,
               NormalizedEmail = registrationRequestDTO.UserName.ToUpper(),
               Name = registrationRequestDTO.Name
            };

            try
            {
                var result = await _userManager.CreateAsync(user,registrationRequestDTO.Password);
                if (result.Succeeded)
                {
                    if(!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult()) {

                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                        await _roleManager.CreateAsync(new IdentityRole("customer"));
                     }
                    await _userManager.AddToRoleAsync(user,"admin");

                    ApplicationUser userToReturn = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == registrationRequestDTO.UserName);

                    return _mapper.Map<UserDTO>(userToReturn);
                }

            }
            catch (Exception e)
            {

                throw;
            }

            return null;
            
        }

        
    }
}
