using Autofac;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz.Spi;

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
            services.AddControllersWithViews();
            services.AddSession();
            services.AddControllers();
            //quartz
            services.AddSingleton<IJobFactory, QuartzJobFactory>();
            ConfigService.InitializeServices(services, Configuration);
            AutofacConfig.ConfigureContainer(services);
            // Create the IServiceProvider based on the container.
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
          //  AutofacConfig.ConfigureContainer(builder);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IRolerInitializerService rolerInitializer
            )
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


            IRolerInitializerService _rolerInitializer= rolerInitializer;
            rolerInitializer.ConfigureInitializer(app);

       //     this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();



        }

    }
}