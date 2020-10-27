using BusinessLayer.Models.BookDTO;
using BusinessLayer.Parser;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IParserBook
    {
        Task<BookViewModel> ParseBookAsync(UrlPicDownload picDownload, long iSBN);
    }
}
