using AutoMedic.DTO;
using AutoMedic.Model;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoMedic.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Users, UserDTO>();
            CreateMap<UserDTO, Users>();
        }
    }
}
