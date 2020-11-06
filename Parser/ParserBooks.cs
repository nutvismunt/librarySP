using BusinessLayer.Interfaces;
using BusinessLayer.Models.BookDTO;
using BusinessLayer.Services;
using BusinessLayer.Services.HttpClientFactory;
using Microsoft.Extensions.Logging;
using Parser.Parser;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parser
{
    public class ParserBooks : IParserBooks
    {
        private readonly ILogger<ParserBooks> _logger;
        private readonly IBookService _bookService;
        private readonly HttpConstructor _constructor;
        public ParserBooks(ILogger<ParserBooks> logger, IBookService bookService,
            HttpConstructor constructor)
        {
            _logger = logger;
            _bookService = bookService;
            _constructor = constructor;
        }

        public async Task<string> ParseBooksAsync(UrlPicDownload picDownload, int amount, long lastISBN)
        {
            var bookNameList = new List<string>();

            var allBooks = _bookService.GetBooks();
            //список из имен существующих книг
            foreach (var onebook in allBooks)
            {
                bookNameList.Add(onebook.BookName);
            }
            var log = "такой книги нет или она уже добавлена";
            try
            {
                var response = await _constructor.CallAsync("/books/" + lastISBN);
                var pageContents = await response.Content.ReadAsStringAsync();
                var pageDocument = new HtmlAgilityPack.HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                // много проверок содержимого тк код с описанием книги меняется от книги к книге
                var bookNameCheck = pageDocument.DocumentNode.SelectSingleNode(".//meta[@name='twitter:title']");
                if (bookNameCheck != null)
                {
                    var bookName = bookNameCheck.Attributes["content"].Value;
                    if (bookName == null) bookName = pageDocument.DocumentNode.SelectSingleNode(".//div[@id='product-title']//h1").InnerText;
                    if (bookNameList.Contains(bookName) == true) goto HtmlParseError;
                    var bookGenre = pageDocument.DocumentNode.SelectSingleNode("(//a[@rel='nofollow']//span[@itemprop='title'])[1]");
                    if (bookGenre == null) goto HtmlParseError;
                    var bookDescription = pageDocument.DocumentNode.SelectSingleNode(".//div[@id='fullannotation']");

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
                    if (bookDescription == null) goto HtmlParseError;
                    var bookPublisher = pageDocument.DocumentNode.SelectSingleNode(".//a[@data-event-label='publisher']").Attributes["data-event-content"].Value;
                    var bookYear = pageDocument.DocumentNode.SelectSingleNode(".//div[@class='publisher']").InnerText;
                    bookYear = bookYear.Remove(bookYear.Length - 3);
                    bookYear = bookYear.Substring(bookYear.Length - 4);
                    var bookAuthor = "Не уточняется";
                    var Author = pageDocument.DocumentNode.SelectSingleNode("//a[@data-event-label='author']");
                    if (Author != null)bookAuthor = Author.Attributes["data-event-content"].Value;
                    if (bookAuthor == null) { bookAuthor = pageDocument.DocumentNode.SelectSingleNode(".//div[@class='authors']//a").InnerText; }
                    var isbnCheck = pageDocument.DocumentNode.SelectSingleNode(".//div[@class='isbn']");
                    var isbn = "";
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
                    var book = new BookViewModel
                    {
                        BookName = bookName,
                        BookDescription = bookDescription.InnerText,
                        BookGenre = bookGenre.InnerText,
                        BookYear = bookYear,
                        BookAuthor = bookAuthor,
                        BookPublisher = bookPublisher,
                        BookInStock = 1,
                        ISBN = ISBN,
                        BookPicName = bookPicName + ".png",
                        BookPicPath = "/Files/" + bookPicName + ".png",
                        WhenAdded = DateTime.Now
                    };
                    _bookService.Create(book);
                    _bookService.Update(book);
                    _logger.LogInformation("book {0} successfully added, bookUrl:{1}", bookName, lastISBN);
                }
                log = lastISBN.ToString();
            }
            catch (Exception loggingException){ _logger.LogInformation(loggingException.ToString());}
        HtmlParseError:
            return log;
        }
    }
}
