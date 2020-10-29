using Autofac;
using AutoMapper;
using BusinessLayer.Configurations;
using BusinessLayer.Interfaces;
using BusinessLayer.Models.JobDTO;
using BusinessLayer.Services;
using BusinessLayer.Services.HttpClientFactory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parser;
using Parser.Jobs;
using Parser.Parser;
using Quartz.Spi;
using System;
using System.Net.Http;

namespace librarySP
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            //for autofac
            var builder = new ConfigurationBuilder()
              .SetBasePath(env.ContentRootPath)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        //for autofac
        public Autofac.IContainer ContextContainer { get; private set; }
        public IConfigurationRoot Configuration { get; private set; }
        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // остальные сервисы находтся в BusinessLayer/Configurations/AutofacConfig
            services.AddControllersWithViews();
            services.AddSession();
            services.AddControllers();
            services.AddTransient(typeof(IParserBook), typeof(ParserBook));
            services.AddTransient(typeof(IParserBooks), typeof(ParserBooks));


            services.AddHttpClient<HttpConstructor>("Labirinth", c =>
             c.BaseAddress = new Uri("https://labirint.ru"));
            services.AddTransient<HttpConstructor>();


            services.AddTransient(typeof(ILabirintBook), typeof(LabitintBook));
            //фиксированное время задачи парсинга книг
            services.AddSingleton<BooksParsingJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(BooksParsingJob),
               cronExpression: "0 44 13 1/1 * ? *")); //каждый день в 17:00: 0 0 17 1/1 * ? * , раз в 6 минут: 0 0/6 * 1/1 * ? *
            //quartz
            services.AddSingleton<IJobFactory, QuartzJobFactory>();
            ConfigService.InitializeServices(services, Configuration);
            AutofacConfig.ConfigureContainer(services);
            // Create the IServiceProvider based on the container.
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, IWebHostEnvironment env,
            IRoleInitializerService roleInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            // инициализация ролей
            IRoleInitializerService role= roleInitializer;
            roleInitializer.ConfigureInitializer(app);
        }

    }
}