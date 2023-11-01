using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dtos;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization.Metadata;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private  IMapper _mapper;
        public VillaController(IVillaService villaService,IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;

            
        }
        public async  Task<ActionResult> IndexVilla()
        {
            List<VillaDTO> list = new();

            var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.IsSuccess) {

                list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
            }
            
            return View(list);
        }

        [Authorize(Roles ="admin")]
        public async Task<ActionResult> CreateVilla()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> CreateVilla(VillaCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.CreateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if(response != null && response.IsSuccess) {
                    TempData["success"] = "Villa added successfully.";
                    return RedirectToAction("IndexVilla");
                }
            }
            TempData["error"] = "Error encountred.";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateVilla(int id) {
        
            var response = await _villaService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.IsSuccess)
            {
                VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
                return View(_mapper.Map<VillaUpdateDTO>(model));
            }
                

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateVilla(VillaUpdateDTO updateDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.UpdateAsync<APIResponse>(updateDTO, HttpContext.Session.GetString(SD.SessionToken));

                if(response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa updated successfully.";
                    return RedirectToAction("IndexVilla");
                }

              

            }
            TempData["error"] = "Error encountred.";
            return View(updateDTO);
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteVilla(int id)
        {
            var response = await _villaService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
                return View(model);
            }


            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteVilla(VillaDTO villaDTO)
        {
            var response = await _villaService.DeleteAsync<APIResponse>(villaDTO.Id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess) {
                TempData["success"] = "Villa added successfully.";
                return RedirectToAction("IndexVilla"); 
            }
            TempData["error"] = "Error encountred.";
            return View(villaDTO);

        }
    }
}
