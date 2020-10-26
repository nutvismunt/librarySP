using Autofac;
using System;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DataLayer.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using DataLayer.Repositories;
using DataLayer.Services;
using Quartz;
using Quartz.Impl;
using BusinessLayer.Services.Jobs;
using BusinessLayer.Models.JobDTO;
using BusinessLayer.Parser;
using AutoMapper;

namespace BusinessLayer.Configurations
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
            services.AddMemoryCache();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddTransient(typeof(ISearchItem<>), typeof(SearchItem<>));
            services.AddTransient(typeof(ISortItem<>), typeof(SortItem<>));
            services.AddTransient(typeof(IUserService), typeof(UserService));
            services.AddTransient(typeof(IBookService), typeof(BookService));
            services.AddTransient(typeof(IOrderService), typeof(OrderService));
            services.AddTransient(typeof(IRoleInitializerService), typeof(RoleInitializerService));
            services.AddTransient(typeof(IReportService), typeof(ReportService));
            services.AddTransient(typeof(ILabirintBook), typeof(LabitintBookId));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //quartz services
            services.AddSingleton(typeof(ISchedulerFactory), typeof(StdSchedulerFactory));
            services.AddHostedService<QuartzHostedService>();
            //quartz job
            services.AddSingleton<AutoCancelOrderJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(AutoCancelOrderJob),
                cronExpression: "0/5 * * * * ?")); //каждые 5 секунд 
            services.AddSingleton<BooksParsingJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(BooksParsingJob),
               cronExpression: "0 34 3 1/1 * ? *")); //каждый день в 17:00 0 0 17 1/1 * ? *     0 0/6 * 1/1 * ? *
            //for autofac
            services.AddOptions();
            services.AddMvc().AddControllersAsServices();

            var container = containerBuilder.Build();
            return container.Resolve<IServiceProvider>();

        }
    }
}
