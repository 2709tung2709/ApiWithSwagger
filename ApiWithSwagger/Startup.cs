using ApiWithSwagger.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace ApiWithSwagger
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(o => o.UseSqlite(_config.GetConnectionString("DataContext")));

            services.AddScoped<IRepository, Repository>();
            services.AddAutoMapper(typeof(Helpers.AutoMapperProfile).Assembly);//Add Auto Mapper profile

            services.AddControllers();

            //Add Swagger
            services.AddSwaggerGen(c=> 
            {
                // Set the comments path for the Swagger JSON and UI.    
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            //Add Swagger with author, license info...
            //services.AddSwaggerGen(c =>
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
   
            //   var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //    c.IncludeXmlComments(xmlPath);
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();//want to custom Swagger UI, enable this

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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
