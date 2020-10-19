using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface IRoleInitializerService
    {
        void ConfigureInitializer(IApplicationBuilder app);
    }
}


