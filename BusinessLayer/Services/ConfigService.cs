using DataLayer;
using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace BusinessLayer.Services
{
    public static class ConfigService
    {
        public static IServiceCollection InitializeServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<LibraryContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<LibraryContext>();
            return services;
        }
    }
}
