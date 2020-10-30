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
using Quartz;
using Quartz.Impl;
using BusinessLayer.Services.Jobs;
using BusinessLayer.Models.JobDTO;
using AutoMapper;

namespace BusinessLayer.Configurations
{
    public class AutofacConfig
    {
        public IConfigurationRoot Configuration { get; private set; }
        public static IServiceProvider ConfigureContainer(IServiceCollection services)
        {

            services.AddMvc();
            // контейнер сервисов
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMemoryCache();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddTransient(typeof(IUserService), typeof(UserService));
            services.AddTransient(typeof(IBookService), typeof(BookService));
            services.AddTransient(typeof(IOrderService), typeof(OrderService));
            services.AddTransient(typeof(IRoleInitializerService), typeof(RoleInitializerService));
            services.AddTransient(typeof(IReportService), typeof(ReportService));

            //quartz services
            services.AddSingleton(typeof(ISchedulerFactory), typeof(StdSchedulerFactory));
            services.AddHostedService<QuartzHostedService>();
            //quartz job
            //авто отмена бронирования спустя 30 минут
            services.AddSingleton<AutoCancelOrderJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(AutoCancelOrderJob),
                cronExpression: "0/5 * * * * ?")); //каждые 5 секунд 

            //for autofac
            services.AddOptions();
            services.AddMvc().AddControllersAsServices();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //построение контейнера для сервисов
            var container = containerBuilder.Build();
            return container.Resolve<IServiceProvider>();

        }
    }
}
