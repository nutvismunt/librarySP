using BusinessLayer.Services.HttpClientFactory;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parser.Parser
{
    public interface IParserBooks
    {
        Task<string> ParseBooksAsync(UrlPicDownload picDownload, int amount, int c, List<string> bookNameList);
    }
}
