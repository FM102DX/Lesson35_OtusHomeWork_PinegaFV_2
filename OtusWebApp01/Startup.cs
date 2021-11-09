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

namespace OtusWebApp01
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.


        private const string RoutePrfix = "MY_CONTEXT_PATH/swagger";
        private const string RouteTemplate = "/swagger/{documentName}/swagger.json";
        private const string SwaggerEndpoint = "/swagger/v1/swagger.json";
        private const string SwaggerTitle = "RFP API";


        public void ConfigureServices(IServiceCollection services)
        {


        //services.AddControllers();
        services.AddControllersWithViews();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OtusWebApp01", Version = "v1" });
            });

            //services.AddTransient<IGenericRepository<Customer>, GenericRepository<Customer>>();

            OtusMultiTreadDbContext context = new OtusMultiTreadDbContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            services.AddSingleton<OtusMultiTreadDbContext>(context);
            services.AddSingleton<IGenericRepository<Customer>, GenericRepository<Customer>>();
            services.AddSingleton<IGenericRepository<ConsoleToApiMessage>, GenericRepository<ConsoleToApiMessage>>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OtusWebApp01 v1"));


            if (env.IsDevelopment())
            {
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger((c =>
            {
                c.RouteTemplate = RoutePrfix + RouteTemplate;
            }));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
