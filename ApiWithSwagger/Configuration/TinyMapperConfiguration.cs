using ApiWithSwagger.Data.Models;
using ApiWithSwagger.Dtos;
using Nelibur.ObjectMapper;
using System.Collections.Generic;

namespace ApiWithSwagger.Configuration
{
    public static class TinyMapperConfiguration
    {
        public static void ConfigureBinding()
        {
            TinyMapper.Bind<List<User>, List<UserDto>>();
            TinyMapper.Bind<UserDto, User>();
        }
    }
}
