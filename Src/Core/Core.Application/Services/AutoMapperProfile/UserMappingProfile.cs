using AutoMapper;
using Core.Application.Models;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Services.AutoMapperProfile
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserLoggedDto>()
             .ForMember(dest => dest.UserId, opt => opt.MapFrom(source => source.Id))
             .ForMember(dest => dest.UserName, opt => opt.MapFrom(source => source.UserName))
             .ForMember(dest => dest.Roles, opt => opt.MapFrom((source, destination) =>
             {
                 var roles = new List<string>();
                 if (source.UserRoles?.Count > 0)
                 {
                     foreach (var role in source.UserRoles)
                     {
                         roles.Add(role.Role.Name);
                     }
                 }

                 return roles;
             }));
        }
    }
}
