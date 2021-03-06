using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Vehicles.Data;
using Microsoft.EntityFrameworkCore;
using Vehicles.Interfaces;
using vehicles.Helpers;
using Vehicles.AuthorizationsManagers;

namespace Vehicles
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            //comment this section to make httpTest work with enshureCreated Db
            #region
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = new VehicleDbContext(
                                services.GetRequiredService<DbContextOptions<VehicleDbContext>>());

                    var appEnvironment = services.GetRequiredService<IWebHostEnvironment>();
                    var seed = new SeedData(new VehicleImageRetriever(),appEnvironment);
                    var userManager = services.GetService<ICustomUserManager>();

                    var roleManager = services.GetRequiredService<ICustomRoleManager>();

                    seed.Initialize(userManager,roleManager, context).Wait();//good way for initializing id set automatically on savechanges
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            #endregion
            //end of section

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
