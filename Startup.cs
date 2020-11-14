using CoinBotCore.Services.UserInfos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using UserDateBase;

namespace Corny_Bot
{
    class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataBaseContext>(options =>
            {
            //options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DataBaseContext;Trusted_Connection=True;MultipleActiveResultSets=true",
            options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DataBaseContext;Trusted_Connection=True;MultipleActiveResultSets=true",
            x => x.MigrationsAssembly("DataBase.Migrations"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddScoped<IUserInfosService, UserInfosService>();

            var serviceProvider = services.BuildServiceProvider();
            var bot = new Bot(serviceProvider);
            services.AddSingleton(bot);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
