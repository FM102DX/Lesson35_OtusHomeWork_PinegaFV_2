using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Otus.Teaching.Concurrency.Import.Core.Entities;
using Otus.Teaching.Concurrency.Import.Core.Repositories;
using Otus.Teaching.Concurrency.Import.DataAccess.Data;
using Otus.Teaching.Concurrency.Import.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtusWebApp02
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OtusWebApp02", Version = "v1" });
            });

            OtusMultiTreadDbContext context = new OtusMultiTreadDbContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();


            /*
             * 
             * services.AddSingleton<OtusMultiTreadDbContext>(context);
            services.AddSingleton<IGenericRepository<Customer>, GenericEFRepository<Customer>>();
            services.AddSingleton<IGenericRepository<ConsoleToApiMessage>, GenericEFRepository<ConsoleToApiMessage>>();


            */

            services.AddScoped<OtusMultiTreadDbContext>();
            services.AddScoped<IGenericRepository<Customer>, GenericEFRepository<Customer>>();
            services.AddScoped<IGenericRepository<ConsoleToApiMessage>, GenericEFRepository<ConsoleToApiMessage>>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {

            }
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OtusWebApp02 v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
