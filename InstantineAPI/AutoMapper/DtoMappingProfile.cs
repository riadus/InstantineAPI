using AutoMapper;
using InstantineAPI.Controllers.Dtos;
using InstantineAPI.Data;

namespace InstantineAPI.AutoMapper
{
    public class DtoMappingProfile : Profile
    {
        public DtoMappingProfile()
        {
            CreateMap<UserDto, User>();
        }
    }
}
