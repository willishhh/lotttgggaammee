using System;
using System.IO;
using System.Reflection;
using LotAPI.Business;
using LotAPI.Business.Interface;
using LotAPI.DataAccess.IsDbContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace LotAPI
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
            services.AddControllers();

            #region Add Swagger service
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "LotAPI",
                    Description = ".Net Core Lot API",
                    Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                });
                string xmlFilePath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                c.IncludeXmlComments(xmlFilePath);
            });
            #endregion

            #region Add Cors service
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader());
            });
            #endregion

            #region Add DbContext
            services.AddDbContext<BasicDbContext>(options =>
            {
                options.UseOracle(Configuration.GetConnectionString("OracleConnection"));
            });
            #endregion

            #region Add Logic Service
            services.AddHttpContextAccessor();
            services.AddScoped<IPrizeDrawLogic, PrizeDrawLogic>();
            #endregion Add Logic Service
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "LotAPI v1");
                c.RoutePrefix = string.Empty;
                c.DisplayRequestDuration();
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
