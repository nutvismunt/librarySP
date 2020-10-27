using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Parser
{
    public interface IParserBooks
    {
        Task<string> ParseBooksAsync(UrlPicDownload picDownload, int amount, int c,List<string> bookNameList);
    }
}
