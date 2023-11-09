using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Models.Dtos
{
    public class UserDTO
    {
        [Key]
        public string Id { get; set; } 
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        
    }
}
