using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using WerewolfCircle.Auth;
using WerewolfCircle.Data;
using WerewolfCircle.Hubs;
using WerewolfCircle.Utils;

namespace WerewolfCircle
{
    public class Startup
    {
        private const string DebugPolicyName = "DebugPolicy";
        private const string GameHubRoute = "/gameHub";

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
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

            services.AddDbContext<GameDbContext>(builder => builder.UseNpgsql(Configuration.GetConnectionString("Default"),
                                                                              b => b.MigrationsAssembly(nameof(WerewolfCircle))));


            IConfiguration jwtConfiguration = Configuration.GetSection("Jwt");
            services.AddOptions<JwtConfig>()
                    .Bind(jwtConfiguration);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        JwtConfig jwtConfig = jwtConfiguration.Get<JwtConfig>();

                        options.RequireHttpsMetadata = !WebHostEnvironment.IsDevelopment();
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            RequireExpirationTime = true,
                            RequireSignedTokens = true,
                            RequireAudience = true,
                            SaveSigninToken = false,
                            TryAllIssuerSigningKeys = true,
                            ValidateActor = false,
                            ValidateAudience = true,
                            ValidateIssuer = true,
                            ValidateIssuerSigningKey = false,
                            ValidateLifetime = true,
                            ValidateTokenReplay = false,
                            ValidIssuer = jwtConfig.Issuer,
                            ValidAudience = jwtConfig.Audience,
                            IssuerSigningKey = jwtConfig.BuildSecretSecurityKey(),
                            ClockSkew = TimeSpan.Zero,
                            // These two things need to be done since we don't want to use Microsofts claim names (see Program.cs).
                            NameClaimType = JwtRegisteredClaimNames.GivenName, // set User.Identity.Name to the player name (which is unique within the game).
                            RoleClaimType = JwtConfig.RoleClaimType // respect the specified role for authorization.
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                StringValues accessToken = context.Request.Query["access_token"];
                                PathString path = context.HttpContext.Request.Path;

                                if (!string.IsNullOrEmpty(accessToken) &&
                                    path.StartsWithSegments(GameHubRoute))
                                {
                                    // Read the token out of the query string (browser limitation for WebSockets)
                                    context.Token = accessToken;
                                }

                                return Task.CompletedTask;
                            }
                        };
                    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.AdminPolicyName, Policies.BuildAdminPolicy());
            });

            services.AddControllers();
            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

            services.AddTransient<IRoomIdGenerator, NanoRoomIdGenerator>();
            services.AddTransient<IAuthTokenGenerator, JwtTokenGenerator>();
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>(GameHubRoute);
            });
        }
    }
}
