using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.Installers;

namespace Vehicles.Installers.Implementations
{
    public static class Installer
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration Configuration) 
        {
            var installers = typeof(Startup).Assembly.ExportedTypes
                .Where(i => typeof(IInstaller).IsAssignableFrom(i) && !i.IsInterface && !i.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IInstaller>()
                .ToList();
            installers.ForEach(installer => installer.InstallServices(services, Configuration));
        }
    }
}
