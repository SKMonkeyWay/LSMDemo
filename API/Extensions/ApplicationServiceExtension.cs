using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtension
    {
         public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ISearchService, SearchService>();
            return services;
        }        
    }
}