using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WerewolfCircle.Data;
using WerewolfCircle.Hubs;

namespace WerewolfCircle
{
    public class Startup
    {
        private const string DebugPolicyName = "DebugPolicy";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(DebugPolicyName, b => b.AllowAnyMethod()
                                                         .AllowAnyHeader()
                                                         .AllowCredentials()
                                                         // only for the vue development server
                                                         .WithOrigins("http://localhost:8080"));
            });

            services.AddDbContext<GameDbContext>(builder => builder.UseSqlite(Configuration.GetConnectionString("Default"),
                                                                              b => b.MigrationsAssembly(nameof(WerewolfCircle))));

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            // During development we want to be able to use the vue dev server
            if (env.IsDevelopment())
            {
                app.UseCors(DebugPolicyName);
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GameHub>("gameHub");
            });
        }
    }
}
