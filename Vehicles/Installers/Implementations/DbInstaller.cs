using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.Data;

namespace Vehicles.Installers.Implementations
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<VehicleDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("VehiclesDbConnection")));
            //services.AddDbContext<VehicleDbContext>(opt =>
            //   opt.UseInMemoryDatabase("VehiclesDbConnection"));//for testing
        }
    }
}
