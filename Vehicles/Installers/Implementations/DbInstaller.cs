﻿using Microsoft.EntityFrameworkCore;
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
using vehicles.Models;


namespace Vehicles.Installers.Implementations
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var server = configuration["DbServer"] ?? "DESKTOP-SH1NLDB\\SQLEXPRESS";
            var port = configuration["DbPort"] ?? "1433"; // Default SQL Server port
            var user = configuration["DbUser"] ?? "sergi_user"; // Warning do not use the SA account
            var password = configuration["Password"] ?? "sergi_password";
            var database = configuration["Database"] ?? "VehiclesWebApiDb";

            services.AddDbContext<VehicleDbContext>(options =>
                options.UseSqlServer(
                    $"Server={server}, {port};Initial Catalog={database};User ID={user};Password={password}"
                    //configuration.GetConnectionString("VehiclesDbConnection")
                    ));

            services.AddDefaultIdentity<CustomUser>()
                .AddRoles<CustomRole>()
                .AddEntityFrameworkStores<VehicleDbContext>();
            //services.AddDbContext<VehicleDbContext>(opt =>
            //   opt.UseInMemoryDatabase("VehiclesDbConnection"));//for testing
        }
    }
}
