using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BoilerPlateCms.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace WebSiteCrawler
{
    class Program
    {
        static void Main(string[] args)
        {

            var services = ConfigureServices();

            var serviceProvider = services.BuildServiceProvider();

            // calls the Run method in App, which is replacing Main
            serviceProvider.GetService<App>().Run();




        }
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();
            services.AddSingleton(config);
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(config.GetConnectionString("DefaultConnection")));
            services.Configure<EmailSettingsModel>(config.GetSection("EmailSettings")); 
            services.AddTransient<App>();
        
            return services;
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }

    }
}
