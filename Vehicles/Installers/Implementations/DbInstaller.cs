using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.Data;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Vehicles.Models;

namespace Vehicles.Installers.Implementations
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<VehicleDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("VehiclesDbConnection")));
            services.AddDefaultIdentity<CustomUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<VehicleDbContext>();
            //services.AddDbContext<VehicleDbContext>(opt =>
            //   opt.UseInMemoryDatabase("VehiclesDbConnection"));//for testing
        }
    }
}
