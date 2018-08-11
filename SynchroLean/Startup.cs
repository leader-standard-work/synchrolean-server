using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SynchroLean.Persistence;
using SynchroLean.Core;
using SynchroLean.Profile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace SynchroLean
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
            services.AddCors();
            services.AddMvc();
            services.AddDbContext<SynchroLeanDbContext>(options => options.UseLazyLoadingProxies()
                                                                          .UseSqlite(Configuration.GetConnectionString("SQLite")));

            // Some research needs to be done in terms of managing instances...
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IUserAccountRepository, UserAccountRepository>();
            services.AddSingleton<IUserTaskRepository, UserTaskRepository>();
            services.AddSingleton<IUserTeamRepository, UserTeamRepository>();
            services.AddSingleton<Rollover>();
            services.AddSingleton<IHostedService, RolloverService>();

            // AutoMapper
            var config = new AutoMapper.MapperConfiguration(c => {
                c.AddProfile(new ApplicationProfile());
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            // JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => 
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    RequireExpirationTime = false,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = "http://localhost:55542",
                    ValidAudience = "http://localhost:4200",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("developmentKey!@3"))
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
            );

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
