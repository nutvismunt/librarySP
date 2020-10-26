using DataLayer.Entities;
using DataLayer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Jobs
{

    public class AutoCancelOrderJob : IJob
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<AutoCancelOrderJob> _logger;

        public AutoCancelOrderJob (IServiceProvider serviceProvider, ILogger<AutoCancelOrderJob> logger)
        {
            _provider = serviceProvider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _provider.CreateScope())
            {
                var dbO = scope.ServiceProvider.GetService<IRepository<Order>>();
                var dbB = scope.ServiceProvider.GetService<IRepository<Book>>();
                var unit = scope.ServiceProvider.GetService<IUnitOfWork>();
                var orders = dbO.GetItems().ToList();
                if (orders != null)
                {
                    foreach (var order in orders.
                        Where(c => c.OrderStatus == 0).
                        Where(c => c.OrderTime.AddMinutes(30) <= DateTime.Now))
                    {
                            dbO.Delete(order);
                            var book = dbB.GetItem(order.BookId);
                            book.BookInStock += order.Amount;
                            unit.Save();
                            _logger.LogInformation("order #{0} deleted", order.Id);
                    }
                }
                await Task.CompletedTask;
            }

        }

    }
}
