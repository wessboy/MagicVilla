using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dtos;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNubmerService _villaNubmerService;
        private readonly IMapper _mapper;
        public VillaNumberController(IVillaNubmerService villaNubmerService,IMapper mapper)
        {
            _mapper = mapper;
            _villaNubmerService = villaNubmerService;
            
        }
        public async  Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO> list = new();
                var response = await _villaNubmerService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {

                list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
                
            }

            return View(list);

        }

        //
    }
}
