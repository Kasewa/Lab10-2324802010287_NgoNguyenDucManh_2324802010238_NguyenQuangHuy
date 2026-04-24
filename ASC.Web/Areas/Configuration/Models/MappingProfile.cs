using AutoMapper;
using ASC.Model.Models;
using ASC.Web.Areas.Configuration.Models;

namespace ASC.Web.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Ánh xạ 2 chiều giữa Entity dưới DB và ViewModel trên Web
            CreateMap<MasterDataKey, MasterDataKeyViewModel>().ReverseMap();
            CreateMap<MasterDataValue, MasterDataValueViewModel>().ReverseMap();
        }
    }
}