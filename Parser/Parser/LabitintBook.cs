using DataLayer.Entities;
using DataLayer.Interfaces;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Parser.Parser
{
    public class LabitintBook : ILabirintBook
    {
        private readonly IRepository<ParserLastUrl> _repository;
        private readonly IUnitOfWork _unitOfWork;
        public LabitintBook(IRepository<ParserLastUrl> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public ParserLastUrl GetParseSettings()
        {
            var settings = _repository.GetItem(1);
            if (settings == null)
            {
                settings = new ParserLastUrl { LastUrl = 773043, BookAmount = 50 };
                _repository.Create(settings);
                _unitOfWork.Save();
            }
            return settings;
        }

        public void UpdateSettings(ParserLastUrl parserLastUrl)
        {
            var id = _repository.GetItem(1);
            var local = _unitOfWork.Context.Set<ParserLastUrl>().Local.
                FirstOrDefault(entry => entry.Id.Equals(id.Id));
            if (local != null)
            {
                _repository.Detatch(local);
            }
            _repository.Update(parserLastUrl);
            _unitOfWork.Save();
        }

        public int GetBookUrl()
        {
            var id = _repository.GetItems();
            var lastUrl = new ParserLastUrl();
            var url = 0;
            if (id.Any() == false)
            {
                lastUrl = new ParserLastUrl { Id=1, LastUrl = 773043, BookAmount = 50 };
                _repository.Create(lastUrl);
                _unitOfWork.Save();
                url = lastUrl.LastUrl;
            }
            else url = id.First().LastUrl;
            return url;
        }

        public void Update(string lastUrl)
        {
            var id = _repository.GetItem(1);
            var local = _unitOfWork.Context.Set<ParserLastUrl>().Local.
                FirstOrDefault(entry => entry.Id.Equals(id.Id));
            if (local != null)
            {
                _repository.Detatch(local);
            }
            var newLastUrl = new ParserLastUrl
            {
                Id = 1,
                LastUrl = int.Parse(lastUrl),
                BookAmount = id.BookAmount
            };
            _repository.Update(newLastUrl);
            _unitOfWork.Save();

        }
    }
}
