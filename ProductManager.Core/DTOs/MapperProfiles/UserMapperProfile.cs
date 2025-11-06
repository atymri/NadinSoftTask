using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.DTOs.UserDTOs;

namespace ProductManager.Core.DTOs.MapperProfiles
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<RegisterRequest, User>();
            CreateMap<LoginRequest, User>();
            CreateMap<User, UserResponse>();
        }
    }
}
