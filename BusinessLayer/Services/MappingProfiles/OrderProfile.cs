using AutoMapper;
using BusinessLayer.Models.BookDTO;
using BusinessLayer.Models.OrderDTO;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
