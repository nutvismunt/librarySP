using AutoMapper;
using BusinessLayer.Models.BookDTO;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Services.MappingProfiles
{
    public class BookProfile :Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookViewModel>();
        }
    }
}
