using AutoMapper;
using MagicVilla_Web.Models.Dtos;

namespace MagicVilla_VillaAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<VillaDTO,VillaCreateDTO>().ReverseMap();
            CreateMap<VillaDTO,VillaUpdateDTO>().ReverseMap();  

            //villa number mapping
          
            CreateMap<VillaNumberDTO,VillaNumberCreatedDTO>().ReverseMap();
            CreateMap<VillaNumberDTO, VillaNumberUpdateDTO>().ReverseMap();
            
        }
    }
}
