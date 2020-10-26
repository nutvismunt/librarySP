using BusinessLayer.Interfaces;
using BusinessLayer.Models.BookDTO;
using BusinessLayer.Parser;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parser
{
    public class ParserBooks : IParserBooks
    {
        private ILogger<ParserBooks> _logger;
        private IBookService _bookService;
        private readonly IHttpClientFactory _httpClientFactory;
        public ParserBooks(ILogger<ParserBooks> logger, IBookService bookService,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _bookService = bookService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> ParseBooksAsync(UrlPicDownload picDownload, int amount, int c,  List<string> bookNameList)
        {
            var log = "такой книги нет или она уже добавлена";
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://www.labirint.ru/books/" + c);
            var pageContents = await response.Content.ReadAsStringAsync();
            var pageDocument = new HtmlAgilityPack.HtmlDocument();
            pageDocument.LoadHtml(pageContents);
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
                    if (Author != null)
                    {
                        bookAuthor = Author.Attributes["data-event-content"].Value;
                    }
                   if (bookAuthor == null) { bookAuthor = pageDocument.DocumentNode.SelectSingleNode(".//div[@class='authors']//a").InnerText; }
                   
                  var bookPic = pageDocument.DocumentNode.SelectSingleNode(".//img[@class='book-img-cover']").Attributes["data-src"].Value;
                
                   
                   var isbn = pageDocument.DocumentNode.SelectSingleNode(".//div[@class='isbn']").InnerText;
                if (isbn.Length >= 24) isbn = isbn.Substring(0, 23);
                  var ISBN = Convert.ToInt64(Regex.Replace(isbn, "[^0-9]", ""));
                   var bookPicName = bookName;
                    if (bookName.Length >= 30) bookPicName = bookName.Substring(0, 30);
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
                        _logger.LogInformation("book {0} successfully added, bookUrl:{1}", bookName, c);
                log = "bookAdded";

            }
            log = c.ToString();
         HtmlParseError:
            return log;

        }
    }
}
