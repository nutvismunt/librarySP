using AutoMapper;
using BusinessLayer.Models.UserDTO;
using DataLayer.Entities;

namespace BusinessLayer.Services.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
        }
    }
}
