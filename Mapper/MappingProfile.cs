using Hajz.Models;
using Hajz.ViewModel;
using AutoMapper;

namespace Hajz.Mapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<CachRE, CachREFormVM>()
                .ReverseMap();

            
            CreateMap<Itemrev, ItemrevFormVM>()
              .ReverseMap();
            CreateMap<Newhajz, NewhajzFormVM>()
              .ReverseMap();
            
        }
    }
}
