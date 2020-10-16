using BusinessLayer.Interfaces;
using DataLayer.Entities;
using DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public void Create(Book book)
        {
            _repository.Create(book);
            _unitOfWork.Save();
        }

        public void Delete(Book book)
        {
            _repository.Delete(book);
            _unitOfWork.Save();
        }

        public Book GetBook(long id)
        {
            return _repository.GetItem(id);
        }

        public IQueryable<Book> GetBooks()
        {
            return _repository.GetItems();
        }

        public List<Book> SearchBook(string searchString)
        {
            return _searchBook.Search(searchString);
        }

        public IQueryable<Book> SortBooks(string sort, bool asc = true)
        {
            return _sortBook.SortedItems(sort);
        }

        public void Update(Book book)
        {
            _repository.Update(book);
            _unitOfWork.Save();
        }
    }
}
