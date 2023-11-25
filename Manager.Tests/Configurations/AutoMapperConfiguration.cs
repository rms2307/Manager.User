using AutoMapper;
using Manager.Domain.Entities;
using Manager.Services.Dtos;

namespace Manager.Tests.Configurations
{
    public static class AutoMapperConfiguration
    {
        public static IMapper GetConfiguration()
        {
            MapperConfiguration mapperConfiguration = new(cfg =>
            {
                cfg.CreateMap<User, UserDto>().ReverseMap();
            });

            return mapperConfiguration.CreateMapper();
        }
    }
}
