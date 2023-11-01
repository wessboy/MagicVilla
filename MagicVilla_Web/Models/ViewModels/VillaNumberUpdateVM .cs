using MagicVilla_Web.Models.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace MagicVilla_Web.Models.ViewModels
{
    public class VillaNumberUpdateVM
    {
        public VillaNumberUpdateVM()
        {
            VillaNumber = new();
        }
        public VillaNumberUpdateDTO VillaNumber { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> VillaList { get; set; }
    }
}
