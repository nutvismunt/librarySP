using BusinessLayer.Interfaces;
using DataLayer.Entities;
using DataLayer.Initializers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Services
{
    public class RolerInitializerService  : IRolerInitializerService
    {

        public async void ConfigureInitializer (IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
    .GetRequiredService<IServiceScopeFactory>()
    .CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await RoleInitializer.InitializeAsync(userManager, rolesManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<RolerInitializerService>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
        }

        public IApplicationBuilder New()
        {
            throw new NotImplementedException();
        }

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            throw new NotImplementedException();
        }
    }
}
