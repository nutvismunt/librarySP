using BusinessLayer.Interfaces;
using BusinessLayer.Models.BookDTO;
using BusinessLayer.Services.HttpClientFactory;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Parser.Parser;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parser
{
    public class ParserBook : IParserBook
    {
        private readonly IBookService _bookService;
        private readonly HttpConstructor _constructor;
        public ParserBook (IBookService bookService, HttpConstructor constructor)
        {
            _bookService = bookService;
            _constructor = constructor;
        }



        public async Task<BookViewModel> ParseBookAsync(UrlPicDownload picDownload, long iSBN)
        {
            var response =await _constructor.CallAsync("/search/"+iSBN);
            var pageContents = await response.Content.ReadAsStringAsync();
            HtmlDocument pageDocument = new HtmlDocument();
            var book = new BookViewModel();
            pageDocument.LoadHtml(pageContents);
            var bookElement = pageDocument.DocumentNode.SelectSingleNode("(//a[@class='cover'])").Attributes["href"].Value;  //получение ссылки на страницу с книгой
            response = await _constructor.CallAsync(bookElement);
            //получение данных о книге
            pageContents = await response.Content.ReadAsStringAsync();
            pageDocument.LoadHtml(pageContents);
            // много проверок содержимого тк код с описанием книги меняется от книги к книге
            var bookNameCheck = pageDocument.DocumentNode.SelectSingleNode(".//meta[@name='twitter:title']");
            var bookName = bookNameCheck.Attributes["content"].Value;
            if (bookName == null) bookName = pageDocument.DocumentNode.SelectSingleNode(".//div[@id='product-title']//h1").InnerText;
            var bookGenre = pageDocument.DocumentNode.SelectSingleNode("(//a[@rel='nofollow']//span[@itemprop='title'])[1]");
            if (bookGenre == null) goto error;
            var bookDescription = pageDocument.DocumentNode.SelectSingleNode(".//div[@id='fullannotation']");

            if (bookDescription == null)
            {
                bookDescription = pageDocument.DocumentNode.SelectSingleNode(".//div[@id='smallannotation']");
                if (bookDescription == null)
                {
                    bookDescription = pageDocument.DocumentNode.SelectSingleNode(".//noindex");
                    if (bookDescription.ToString().Length < 30) bookDescription = pageDocument.DocumentNode.SelectSingleNode(".//div[@id='product-about']//p");
                }
            }
            if (bookDescription == null) goto error;
            var bookPublisher = pageDocument.DocumentNode.SelectSingleNode(".//a[@data-event-label='publisher']").Attributes["data-event-content"].Value;
            var bookYear = pageDocument.DocumentNode.SelectSingleNode(".//div[@class='publisher']").InnerText;
            bookYear = bookYear.Remove(bookYear.Length - 3);
            bookYear = bookYear.Substring(bookYear.Length - 4);
            var bookAuthor = "Не уточняется";
            var Author = pageDocument.DocumentNode.SelectSingleNode("//a[@data-event-label='author']");
            if (Author != null)
            {
                bookAuthor = Author.Attributes["data-event-content"].Value;
            }
            if (bookAuthor == null) { bookAuthor = pageDocument.DocumentNode.SelectSingleNode(".//div[@class='authors']//a").InnerText; }
            var isbnCheck = pageDocument.DocumentNode.SelectSingleNode(".//div[@class='isbn']");
            string isbn;
            if (isbnCheck != null) isbn = isbnCheck.InnerText;
            else isbn = "0";
            if (isbn.Length >= 24) isbn = isbn.Substring(0, 23);
            var ISBN = Convert.ToInt64(Regex.Replace(isbn, "[^0-9]", ""));
            var bookPicCheck = pageDocument.DocumentNode.SelectSingleNode(".//img[@class='book-img-cover']").Attributes["data-src"];
            var bookPic = "https://img.labirint.ru/design/emptycover.png";
            if (bookPicCheck == null) bookPicCheck = pageDocument.DocumentNode.SelectSingleNode(".//img[@class='book-img-cover']").Attributes["src"];
            if (bookPicCheck != null) bookPic = bookPicCheck.Value;
            var bookPicName = Guid.NewGuid().ToString().Substring(0, 30);
            picDownload.SaveImage("wwwroot//Files//" + bookPicName + ".png", bookPic);

            book = new BookViewModel
            {
                BookName = bookName,
                BookDescription = bookDescription.InnerText,
                BookGenre = bookGenre.InnerText,
                BookYear = bookYear,
                BookAuthor = bookAuthor,
                BookPublisher = bookPublisher,
                BookInStock = 0,
                ISBN = iSBN,
                BookPicName = bookPicName + ".png",
                BookPicPath = "/Files/" + bookPicName + ".png"
            };
            _bookService.Create(book);
        error:
            return book;

        }

    }
}
