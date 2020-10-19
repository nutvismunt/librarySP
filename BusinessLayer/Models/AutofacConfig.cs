using Autofac;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac.Core;
using System.Web;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using DataLayer.Entities;
using BusinessLayer.Models.UserDTO;
using BusinessLayer.Models.BookDTO;
using BusinessLayer.Models.OrderDTO;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DataLayer.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using System.Configuration;
using DataLayer.Repositories;
using DataLayer.Services;
using Quartz;
using Quartz.Impl;
using BusinessLayer.Services.Jobs;
using BusinessLayer.Models.JobDTO;

namespace BusinessLayer.Models
{
    public class AutofacConfig
    {
        public IConfigurationRoot Configuration { get; private set; }
        public static IServiceProvider ConfigureContainer(IServiceCollection services)
        {

            services.AddMvc();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc();
            services.AddMemoryCache();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddTransient(typeof(ISearchItem<>), typeof(SearchItem<>));
            services.AddTransient(typeof(ISortItem<>), typeof(SortItem<>));
            services.AddTransient(typeof(IUserService), typeof(UserService));
            services.AddTransient(typeof(IBookService), typeof(BookService));
            services.AddTransient(typeof(IOrderService), typeof(OrderService));
            services.AddTransient(typeof(IRolerInitializerService), typeof(RolerInitializerService));
            services.AddTransient(typeof(User), typeof(UserViewModel));
            //quartz services
            services.AddSingleton(typeof(ISchedulerFactory), typeof(StdSchedulerFactory));
            services.AddHostedService<QuartzHostedService>();
            //quartz job
            services.AddSingleton<AutoCancelOrderJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(AutoCancelOrderJob),
                cronExpression: "0/5 * * * * ?")); //каждые 30 минут, "0/5 * * * * ?" 5 секунд
            //for autofac
            services.AddOptions();
            services.AddMvc().AddControllersAsServices();

            var container = containerBuilder.Build();
            return container.Resolve<IServiceProvider>();

        }
    }
}
