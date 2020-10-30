using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLayer.Interfaces;
using BusinessLayer.Models.BookDTO;
using DataLayer.Entities;
using DataLayer.Interfaces;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods.IQueryableExtensions;

namespace BusinessLayer.Services
{
    public class BookService : IBookService 
    {
        private readonly IRepository<Book> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(IRepository<Book> repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
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
            var book = _mapper.Map<List<BookViewModel>>(_repository.GetItems().ProjectTo<BookViewModel>(_mapper.ConfigurationProvider));         
            return book.AsQueryable();
        }

        public List<BookViewModel> SearchBook(string searchString)
        {
            var query = _repository.GetItems().Search(searchString);
            var searcher = _mapper.Map<List<BookViewModel>>(query);
            return searcher;
        }

        public IQueryable<BookViewModel> SortBooks(string sort, bool asc)
        {
            var query = _repository.GetItems().SortedItems(sort, asc);
            var sorter= _mapper.Map<List<BookViewModel>>(query.ProjectTo<BookViewModel>(_mapper.ConfigurationProvider));
            return sorter.AsQueryable();
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
