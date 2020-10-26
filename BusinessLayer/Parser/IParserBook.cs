using BusinessLayer.Models.BookDTO;
using BusinessLayer.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IParserBook
    {
        Task<BookViewModel> ParseBookAsync(UrlPicDownload picDownload, long iSBN);
    }
}
