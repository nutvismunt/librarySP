using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface IRoleInitializerService
    {
        // инициализатор ролей в бд
        void ConfigureInitializer(IApplicationBuilder app);
    }
}


