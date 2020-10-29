using BusinessLayer.Models.BookDTO;
using System.Threading.Tasks;

namespace Parser.Parser
{
    public interface IParserBook
    {
        Task<BookViewModel> ParseBookAsync(UrlPicDownload picDownload, long iSBN);
    }
}
