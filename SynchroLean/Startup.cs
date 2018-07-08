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

            services.AddDbContext<SynchroLeanDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("SQLite")));

            // Some research needs to be done in terms of managing instances...
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IUserAccountRepository, UserAccountRepository>();
            services.AddSingleton<IUserTaskRepository, UserTaskRepository>();
            services.AddSingleton<IUserTeamRepository, UserTeamRepository>();
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
