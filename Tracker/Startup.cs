using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Tracker {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { private set; get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddScoped<Microsoft.EntityFrameworkCore.Infrastructure.IDbContextOptions, DbContextOptions<TrackerContext>>();
            services.AddControllers();
            services.AddCors();
            services.AddMvc(c => c.Conventions.Add(new ApiExplorerIgnores()));
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Tracker", Version = "v1"}); });
            services.AddDbContext<TrackerContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("TrackerContext"),
                    new MySqlServerVersion(new Version(10, 5, 9))));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tracker v1"));
            }

            app.Use(async (context, next) =>
            {
                context.Response.OnStarting(async o => {
                    if (o is HttpContext ctx) {
                        ctx.Response.Headers["Access-Control-Allow-Origin"] = "*";
                    }
                }, context);
                await next();
            });
            app.UseRouting();
            
            app.UseAuthorization();

            app.UseAuthentication();
            
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }

    public class ApiExplorerIgnores : IActionModelConvention {
        public void Apply(ActionModel action)
        {
            if (action.Controller.ControllerName.Equals("Pwa"))
                action.ApiExplorer.IsVisible = false;
        }
    }
}