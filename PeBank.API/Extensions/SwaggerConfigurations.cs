using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PeBank.API.Extensions
{
    public static class SwaggerConfigurations
    {
        public static void ConfigureSwagger(this IServiceCollection services, IConfiguration config)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Pe Bank",
                    Description = "Pe Bank's API",
                    
                });

                options.SchemaFilter<ArraySchemaFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlFeaturesFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.Features.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFeaturesFile));
            });
        }

        public static void UseSwaggerV1(this IApplicationBuilder app, IConfiguration config)
        {
            var appName = config["App:Name"];
            var queryString = new Dictionary<string, string>();
            queryString.Add("resource", config["AzureAD:ClientId"]);
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", $"Pe Bank V1");
            });
        }
    }

    public class ArraySchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsArray && context.Type.HasElementType)
            {
                var elemType = context.Type.GetElementType();

                var name = elemType.Name + "Array";
                var elemSchema = context.SchemaRepository.Schemas.GetValueOrDefault(elemType.Name);
                var description = elemSchema != null ? "Array of " + elemSchema.Description : null;
                if (!context.SchemaRepository.Schemas.ContainsKey(name))
                {
                    context.SchemaRepository.Schemas.Add(
                        name,
                        new OpenApiSchema
                        {
                            UniqueItems = schema.UniqueItems,
                            Type = schema.Type,
                            Items = schema.Items,
                            Description = description
                        });
                }

                schema.UniqueItems = null;
                schema.Type = null;
                schema.Items = null;
                schema.Description = description;
                schema.Reference = new OpenApiReference
                {
                    Id = name,
                    Type = ReferenceType.Schema
                };
            }
        }
    }
}
