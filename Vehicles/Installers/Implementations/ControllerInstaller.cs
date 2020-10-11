﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SystemTextJsonSamples;
using Vehicles.Interfaces.RepositoryInterfaces;
using Vehicles.Interfaces.ServiceInterfaces;
using Vehicles.Repositories;
using Vehicles.Services;
using Microsoft.AspNetCore.Http;
using Vehicles.MyCustomMapper;

namespace Vehicles.Installers.Implementations
{
    public class ControllerInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddCors(options =>
            {
                options.AddPolicy(name: Startup.MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:4500")
                                                          .AllowAnyHeader()
                                                          .AllowAnyMethod();
                                      //.WithHeaders(HeaderNames.Accept, "application/json")
                                      //.WithHeaders(HeaderNames.ContentType, "application/json")
                                      //.WithMethods("PUT", "POST", "DELETE", "GET", "PATCH");
                                  });
            });
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.Converters.Add(new DecimalToStringConverter());
                options.JsonSerializerOptions.Converters.Add(new FloatToStringConverter());
                options.JsonSerializerOptions.Converters.Add(new IntToStringConverter());
            });


            services.AddTransient<ICarOwnersRepository, CarOwnersRepository>();
            services.AddTransient<ICarsRepository, CarsRepository>();

            services.AddTransient<ICarOwnerService, CarOwnerService>();
            services.AddTransient<ICarService, CarService>();
            services.AddTransient<ICustomMapper,CustomMapper>();

            services.AddSingleton<IUriService>(provider=>{
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;//http or https
                var absoluteUri = $"{request.Scheme}://{request.Host.ToUriComponent()}/";
                return new UriService(absoluteUri);
            });
        }
    }
}
