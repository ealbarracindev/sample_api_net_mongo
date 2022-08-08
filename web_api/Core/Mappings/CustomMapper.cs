using AutoMapper;
using web_api.Core.Dtos;
using web_api.Core.Models;

namespace web_api.Core.Mappings
{
    public class CustomMapper : Profile
    {
        public CustomMapper()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }

}
