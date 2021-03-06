using FluentValidation.AspNetCore;
using Hahn.ApplicatonProcess.May2020.Data.Base;
using Hahn.ApplicatonProcess.May2020.Data.DbContext;
using Hahn.ApplicatonProcess.May2020.Data.Repositories;
using Hahn.ApplicatonProcess.May2020.Domain.Interfaces;
using Hahn.ApplicatonProcess.May2020.Domain.Managers;
using Hahn.ApplicatonProcess.May2020.Domain.Mappers;
using Hahn.ApplicatonProcess.May2020.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Reflection;

namespace Hahn.ApplicatonProcess.May2020.Api
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc().AddFluentValidation();
            services.AddHttpClient();
            services.AddTransient<IValidationManager, ValidationManager>();
            services.AddTransient<IApplicantManager, ApplicantManager>();
            services.AddTransient<IErrorMapper, ErrorMapper>();
            services.AddTransient<IGenericRepository<Applicant>, GenericRepository<Applicant>>();
            services.AddTransient<ApplicantRepository>();
            services.AddDbContext<HahnDbContext>(options => options.UseInMemoryDatabase("HahnDbContext"));
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1",
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "Swagger API",
                        Description = "API for Hahn Applicant Process",
                        Version = "v1"
                    });
                var fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var filePath = Path.Combine(AppContext.BaseDirectory,fileName);
                options.IncludeXmlComments(filePath);
            });
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(options=> {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hahn Application Process API");
            
            });
            //app.UseCors("CorsPolicy");
        }
    }
}
