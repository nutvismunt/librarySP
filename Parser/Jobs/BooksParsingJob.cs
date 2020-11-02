﻿using BusinessLayer.Interfaces;
using BusinessLayer.Services.HttpClientFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Parser.Parser;
using Quartz;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parser.Jobs
{
    public class BooksParsingJob : IJob
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<BooksParsingJob> _logger;
        private readonly HttpConstructor _httpConstructor;

        public BooksParsingJob(IServiceProvider serviceProvider, ILogger<BooksParsingJob> logger,
            HttpConstructor httpConstructor)
        {
            _provider = serviceProvider;
            _logger = logger;
            _httpConstructor = httpConstructor;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _provider.CreateScope())
            {
                var bookService = scope.ServiceProvider.GetService<IBookService>();
                var parse = scope.ServiceProvider.GetService<IParserBooks>();
                var allBooks = bookService.GetBooks();
                var booksNameList = new List<string>();
                var labId = scope.ServiceProvider.GetService<ILabirintBook>();
                //список из имен существующих книг
                foreach (var onebook in allBooks)
                {
                    booksNameList.Add(onebook.BookName);
                }
                var pic = new UrlPicDownload();
                //полчение количества книг, которые нужно добавить
                var amount = labId.GetParseSettings().BookAmount;
                var str = "";
                //id книги
                var c = labId.GetBookUrl();
                for (var i = 0; i < amount; i++)
                {
                    str = await parse.ParseBooksAsync(pic, amount, c, booksNameList);
                    //+1 выполнение цикла, если книга не найдена
                    if (str == "такой книги нет или она уже добавлена") amount++;
                    c--;
                }
                // обновить url последней книги в бд
                labId.Update(str);
                _logger.LogInformation("book parsing succesfully completed");
            }
            await Task.CompletedTask;
        }
    }
}