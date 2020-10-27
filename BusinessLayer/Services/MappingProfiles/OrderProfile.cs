using AutoMapper;
using BusinessLayer.Models.OrderDTO;
using DataLayer.Entities;

namespace BusinessLayer.Services.MappingProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile ()
        {
            CreateMap<Order, OrderViewModel>();
        }
    }
}
