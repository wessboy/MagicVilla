using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dtos;
using MagicVilla_Web.Models.ViewModels;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNubmerService _villaNubmerService;
        private readonly IMapper _mapper;
        private readonly IVillaService _villaService;
        public VillaNumberController(IVillaNubmerService villaNubmerService, IMapper mapper, IVillaService villaService)
        {
            _mapper = mapper;
            _villaNubmerService = villaNubmerService;
            _villaService = villaService;
        }
        [Authorize]
        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO> list = new();
            var response = await _villaNubmerService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {

                list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));

            }

            return View(list);

        }

        //
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> CreateVillaNumber()
        {
            VillaNumberCreateVM villaNumberCreateVM = new();
            var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {

                villaNumberCreateVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result)).
                       Select(v => new SelectListItem
                       {
                           Text = v.Name,
                           Value = v.Id.ToString(),
                       });


            }

            return View(villaNumberCreateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> CreateVillaNumber(VillaNumberCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNubmerService.CreateAsync<APIResponse>(model.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa Number added successfully.";
                    return RedirectToAction("IndexVillaNumber");
                }
                else
                {
                    if(response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages",response.ErrorMessages.FirstOrDefault());
                    }
                }
            }

            // populate the list of villa again in case of an error

            var responseAfterError = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (responseAfterError != null && responseAfterError.IsSuccess)
            {

                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(responseAfterError.Result)).
                       Select(v => new SelectListItem
                       {
                           Text = v.Name,
                           Value = v.Id.ToString(),
                       });
            }

            TempData["error"] = "Error encountred.";
            return View(model);
            
        }

        [Authorize(Roles = "admin")]
        public async  Task<ActionResult> UpdateVillaNumber(int villaNo)
        {
            VillaNumberUpdateVM villaNumberUpdateVM = new();
            var response = await _villaNubmerService.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.IsSuccess)
            {
                VillaNumberDTO villaNumber = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                villaNumberUpdateVM.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);
            }

            response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.IsSuccess)
            {
                villaNumberUpdateVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result)).Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString(),
                });
                return View(villaNumberUpdateVM);
            }

            return NotFound();
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
        {
            if(ModelState.IsValid)
            {
                var response = await _villaNubmerService.UpdateAsync<APIResponse>(model.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));
                if(response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa Number updated successfully.";
                    return RedirectToAction("IndexVillaNumber");
                }
                else
                {
                    if (response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                    }
                }
            }

            //
            var responseAfterError = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (responseAfterError != null && responseAfterError.IsSuccess)
            {

                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(responseAfterError.Result)).
                       Select(v => new SelectListItem
                       {
                           Text = v.Name,
                           Value = v.Id.ToString(),
                       });
            }
            TempData["error"] = "Error encountred.";
            return View(model);

            
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteVillaNumber(int villaNo)
        {
            VillaNumberDeleteVM villaNumberDeleteVM = new();
            var response = await  _villaNubmerService.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.IsSuccess)
            {
                villaNumberDeleteVM.VillaNumber =JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
            }

            response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.IsSuccess)
            {
                villaNumberDeleteVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result)).
                       Select(v => new SelectListItem
                       {
                           Text = v.Name,
                           Value = v.Id.ToString(),
                       });

                return View(villaNumberDeleteVM);
            }

            return NotFound();  
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteVillaNumber(VillaNumberDeleteVM villaNumberDeleteVM)
        {
            var response = await _villaNubmerService.DeleteAsync<APIResponse>(villaNumberDeleteVM.VillaNumber.VillaNo, HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.IsSuccess)
            {
                
                TempData["success"] = "Villa Number deleted successfully.";
                return RedirectToAction("IndexVillaNumber");
            }
            TempData["error"] = "Error encountred.";
            return View(villaNumberDeleteVM);
        }

       


        /*controller end */
    }
}
