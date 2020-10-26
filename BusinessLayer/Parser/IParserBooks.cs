using BusinessLayer.Models.BookDTO;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Parser
{
    public interface IParserBooks
    {
        Task<string> ParseBooksAsync(UrlPicDownload picDownload, int amount, int c,List<string> bookNameList);
    }
}
