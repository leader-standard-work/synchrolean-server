using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SynchroLean.Persistence;
using SynchroLean.Core;

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
            services.AddMvc();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUserAccountRepository, UserAccountRepository>();
            services.AddSingleton<IUserAccountRepository, UserAccountRepository>();
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            services.AddTransient<IUserTaskRepository, UserTaskRepository>();
            services.AddSingleton<IUserTaskRepository, UserTaskRepository>();
            services.AddScoped<IUserTaskRepository, UserTaskRepository>();
            services.AddTransient<IUserTeamRepository, UserTeamRepository>();
            services.AddSingleton<IUserTeamRepository, UserTeamRepository>();
            services.AddScoped<IUserTeamRepository, UserTeamRepository>();
            services.AddDbContext<SynchroLeanDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("SQLite")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
