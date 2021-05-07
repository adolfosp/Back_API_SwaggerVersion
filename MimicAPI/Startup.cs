using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MimicAPI.V1.Repositories;
using MimicAPI.V1.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MimicAPI.Helpers;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace MimicAPI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region AUTOMAPPER
            var config =new MapperConfiguration(cfg => {
                cfg.AddProfile( new DTOMapperProfile());
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            services.AddDbContext<Database.MimicContext>(opt =>
            {
                opt.UseSqlite("Data Source=Database\\Mimic.db");
            });
            services.AddMvc();
            services.AddScoped<IPalavraRepository, PalavraRepository>();
            services.AddApiVersioning(cfg =>
            {

                cfg.ReportApiVersions = true;
                //cfg.AssumeDefaultVersionWhenUnspecified = true;
                //cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Awesome CMS Core API V1"
                    
                });
                c.SwaggerDoc("v1.1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1.1",
                    Title = "Awesome CMS Core API V1.1",

                });
                c.SwaggerDoc("v2", new  Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v2",
                    Title = "Awesome CMS Core API V2",
                    
                });
                var caminhoPorjeto = PlatformServices.Default.Application.ApplicationBasePath;
                var nomeProjeto = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var caminhoTotalDoArquivo = Path.Combine(caminhoPorjeto, nomeProjeto);
                c.IncludeXmlComments(caminhoTotalDoArquivo);
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStatusCodePages();
            

            app.UseEndpoints(endpoints =>
            {
               
                     endpoints.MapControllers();

               
            });
           

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", "Awesome CMS Core API V1");
                c.SwaggerEndpoint($"/swagger/v1.1/swagger.json", "Awesome CMS Core API V1.1");
                c.SwaggerEndpoint($"/swagger/v2/swagger.json", "Awesome CMS Core API V2");
                c.RoutePrefix = String.Empty;
            });



        }
    }
}
