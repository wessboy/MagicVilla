using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dtos
{
    public class VillaNumberCreatedDTO
    {
        [Required]
        public int VillaNo { get; set; }
        public string SpecialDetails { get; set; }
        [Required]
        public int VillaId { get; set; }
    }
}
