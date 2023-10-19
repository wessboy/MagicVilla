using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dtos;

namespace MagicVilla_VillaAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa,VillaDTO>().ReverseMap();
            CreateMap<Villa,VillaCreateDTO>().ReverseMap();
            CreateMap<Villa,VillaUpdateDTO>().ReverseMap();  

            //villa number mapping
            CreateMap<VillaNumber,VillaNumberDTO>().ReverseMap();
            CreateMap<VillaNumber,VillaNumberCreatedDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();
            
        }
    }
}
