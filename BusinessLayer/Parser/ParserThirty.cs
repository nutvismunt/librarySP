using BusinessLayer.Interfaces;
using BusinessLayer.Models.BookDTO;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Parser
{
    public class ParserThirty //: IParserThirty
    {
      /*  private ILogger<Parser> _logger;
        private IBookService _bookService;
        public ParserThirty(ILogger<Parser> logger, IBookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        public async Task<BookViewModel> ParseAsync(UrlPicDownload picDownload, long iSBN)
        {
            HttpClient client = new HttpClient();
            //запрос поиска по isbn
            var response = await client.GetAsync("https://www.labirint.ru/search/" + iSBN);
            var pageContents = await response.Content.ReadAsStringAsync();
            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);
            var bookElement = pageDocument.DocumentNode.SelectSingleNode("(//a[@class='cover'])").Attributes["href"].Value;  //получение ссылки на страницу с книгой
            response = await client.GetAsync("https://www.labirint.ru" + bookElement);
            //получение данных о книге
            pageContents = await response.Content.ReadAsStringAsync();
            pageDocument.LoadHtml(pageContents);
            var bookName = pageDocument.DocumentNode.SelectSingleNode(".//meta[@name='twitter:title']").Attributes["content"].Value;
            var bookGenre = pageDocument.DocumentNode.SelectSingleNode(".//span[@itemprop='title']").InnerText;
            HtmlNode bookDescription = (pageDocument.DocumentNode.SelectSingleNode(".//div[@id='fullannotation']"));

            if (bookDescription == null)
            {
                bookDescription = pageDocument.DocumentNode.SelectSingleNode(".//div[@id='smallannotation']");
                if (bookDescription == null)
                {
                    bookDescription = pageDocument.DocumentNode.SelectSingleNode(".//noindex");
                    if (bookDescription.ToString().Length < 30)
                    {
                        bookDescription = pageDocument.DocumentNode.SelectSingleNode(".//div[@id='product-about']//p");
                    }
                }
            }
            var bookPublisher = pageDocument.DocumentNode.SelectSingleNode(".//a[@data-event-label='publisher']").Attributes["data-event-content"].Value;
            var bookYear = pageDocument.DocumentNode.SelectSingleNode(".//div[@class='publisher']").InnerText;
            bookYear = bookYear.Remove(bookYear.Length - 3);
            bookYear = bookYear.Substring(bookYear.Length - 4);
            var bookAuthor = pageDocument.DocumentNode.SelectSingleNode("//a[@data-event-label='author']").Attributes["data-event-content"].Value;
            var bookPic = pageDocument.DocumentNode.SelectSingleNode(".//img[@class='book-img-cover']").Attributes["data-src"].Value;
            picDownload.SaveImage("wwwroot//Files//" + bookName + ".png", bookPic);

            var book = new BookViewModel
            {
                BookName = bookName,
                BookDescription = bookDescription.InnerText,
                BookGenre = bookGenre,
                BookYear = bookYear,
                BookAuthor = bookAuthor,
                BookPublisher = bookPublisher,
                BookInStock = 0,
                ISBN = iSBN,
                BookPicName = bookName + ".png",
                BookPicPath = "/Files/" + bookName + ".png"
            };
            _bookService.Create(book);

            return book;

        }*/
    }
}
