using AutoMapper;
using BusinessLayer.Interfaces;
using BusinessLayer.Models.BookDTO;
using DataLayer.Entities;
using DataLayer.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Services
{
    public class BookService : IBookService 
    {
        private readonly IRepository<Book> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISearchItem<Book> _searchBook;
        private readonly ISortItem<Book> _sortBook;
        private readonly IMapper _mapper;

        public BookService(IRepository<Book> repository, ISearchItem<Book> searchBook,
            ISortItem<Book> sortBook, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _searchBook = searchBook;
            _sortBook = sortBook;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Create(BookViewModel book)
        {
            _repository.Create(_mapper.Map<Book>(book));
            _unitOfWork.Save();
        }

        public void Delete(Book book)
        {
            var local = _unitOfWork.Context.Set<Book>().Local.
                FirstOrDefault(entry => entry.Id.Equals(book.Id));
            if (local != null)
            {
                _repository.Detatch(local);
            }
            _repository.Delete(book);
            _unitOfWork.Save();
        }

        public BookViewModel GetBook(long id)
        {
            return _mapper.Map<BookViewModel>(_repository.GetItem(id));
        }

        public IQueryable<BookViewModel> GetBooks()
        {
            var book = _mapper.Map<List<BookViewModel>>(_repository.GetItems().ToList());
            var books = book.AsQueryable();
            return books;
        }

        public List<BookViewModel> SearchBook(string searchString)
        {
            var book = _searchBook.Search(searchString);
            return _mapper.Map<List<BookViewModel>>(book);
        }

        public IQueryable<BookViewModel> SortBooks(string sort, bool asc)
        {
            var sorter = _mapper.Map<List<BookViewModel>>(_sortBook.SortedItems(sort, asc).ToList());
            var books = sorter.AsQueryable();
            return books;
        }

        public void Update(BookViewModel book)
        {
            var local = _unitOfWork.Context.Set<Book>().Local.
                FirstOrDefault(entry => entry.Id.Equals(book.Id));
            if (local != null) _repository.Detatch(local);
            _repository.Update(_mapper.Map<Book>(book));
            _unitOfWork.Save();
        }
    }
}
