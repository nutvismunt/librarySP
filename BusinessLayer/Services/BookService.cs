using Autofac;
using Autofac.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLayer.Interfaces;
using BusinessLayer.Models.BookDTO;
using DataLayer.Entities;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class BookService : IBookService 
    {
        private readonly IRepository<Book> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISearchItem<Book> _searchBook;
        private readonly ISortItem<Book> _sortBook;

        public BookService(IRepository<Book> repository, ISearchItem<Book> searchBook, ISortItem<Book> sortBook, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _searchBook = searchBook;
            _sortBook = sortBook;
            _unitOfWork = unitOfWork;
        }

        public void Create(BookViewModel book)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Book, BookViewModel>()).CreateMapper();
            _repository.Create(config.Map<Book>(book));
            _unitOfWork.Save();
        }

        public void Delete(Book book)
        {
            _repository.Delete(book);
            _unitOfWork.Save();
        }

        public BookViewModel GetBook(long id)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Book, BookViewModel>()).CreateMapper();
            return config.Map<Book, BookViewModel>(_repository.GetItem(id));
        }

        public IQueryable<BookViewModel> GetBooks()
        {
            var config = new MapperConfiguration(cfg =>  cfg.CreateMap<Book, BookViewModel>()).CreateMapper();
            var book = config.Map<List<Book>, List<BookViewModel>>(_repository.GetItems().ToList());
            var books = book.AsQueryable();
            return books;
        }

        public List<BookViewModel> SearchBook(string searchString)
        {
            var book = _searchBook.Search(searchString);
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Book, BookViewModel>()).CreateMapper();
            return config.Map<List<BookViewModel>>(book);
        }

        public IQueryable<BookViewModel> SortBooks(string sort, bool asc)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Book, BookViewModel>()).CreateMapper();
            var sorter = config.Map<List<Book>, List<BookViewModel>>(_sortBook.SortedItems(sort,asc).ToList());
            var books = sorter.AsQueryable();

            return books;
        }

        public void Update(BookViewModel book)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Book, BookViewModel>()).CreateMapper();
            _repository.Update(config.Map<Book>(book));
            _unitOfWork.Save();
        }
    }
}
