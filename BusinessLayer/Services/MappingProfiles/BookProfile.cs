using AutoMapper;
using BusinessLayer.Models.BookDTO;
using DataLayer.Entities;

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
