using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace PeBank.API.Extensions
{
    public static class AutoMapperConfigurations
    {
        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            var assemblies = new List<Assembly> { Assembly.Load("PeBank.API.Features") };

            services.AddAutoMapper(assemblies);
        }
    }
}
