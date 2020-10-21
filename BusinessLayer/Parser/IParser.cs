using BusinessLayer.Models.BookDTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Parser
{
    public interface IParser
    {
        Task<BookViewModel> ParseAsync (UrlPicDownload picDownload, long iSBN);
    }
}
