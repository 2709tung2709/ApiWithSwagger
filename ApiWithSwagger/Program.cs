using ApiWithSwagger.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System;
using Microsoft.Extensions.Hosting;
using Nelibur.ObjectMapper;
using System.Collections.Generic;
using ApiWithSwagger.Dtos;
using ApiWithSwagger.Configuration;

namespace ApiWithSwagger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

            //add services to the container
            builder.Services.AddDbContext<DataContext>(x => x.UseSqlite(config.GetConnectionString("DataContext")));
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(c =>
            {
                // Set the comments path for the Swagger JSON and UI.    
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            builder.Services.AddScoped<IRepository, Repository>();
            //Add Swagger with author, license info...
            //builder.Services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Version = "v1",
            //        Title = "API With Swagger",
            //        Description = ".NET API with Swagger documentation example",
            //        TermsOfService = new Uri("https://example.com/terms"),
            //        Contact = new OpenApiContact
            //        {
            //            Name = "Tung Nguyen",
            //            Email = string.Empty,
            //            Url = new Uri("https://twitter.com/someone"),
            //        },
            //        License = new OpenApiLicense
            //        {
            //            Name = "Use under LICX",
            //            Url = new Uri("https://example.com/license"),
            //        }
            //    });

            //    //Set the comments path for the Swagger JSON and UI.

            //    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //    c.IncludeXmlComments(xmlPath);
            //});

            TinyMapperConfiguration.ConfigureBinding();

            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                //
            }

            //want to custom Swagger UI, enable this
            //app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API");
                c.RoutePrefix = string.Empty; // enabled :https://<hostname>/  disabled: https://<hostname>/swagger
                //c.InjectStylesheet("/swagger-ui/custom.css"); // Stylesheet for custom UI
            });

            app.UseRouting();
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.MapControllers();

            app.Run();
        }
    }
}
