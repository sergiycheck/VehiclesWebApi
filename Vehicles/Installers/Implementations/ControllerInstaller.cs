using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SystemTextJsonSamples;
using Vehicles.Interfaces.RepositoryInterfaces;
using Vehicles.Interfaces.ServiceInterfaces;
using Vehicles.Repositories;
using Vehicles.Services;
using Microsoft.AspNetCore.Http;
using Vehicles.MyCustomMapper;
using Vehicles.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using  Vehicles.Interfaces;
using Vehicles.AuthorizationsManagers;
using Vehicles.AuthorizationsManagers.Attributes;
using vehicles.Helpers;
using Microsoft.AspNetCore.Http.Features;

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
                                      builder.WithOrigins("http://localhost:4500", 
                                                          "http://localhost:3000")
                                                          .AllowAnyHeader()
                                                          .AllowAnyMethod()
                                                          .AllowCredentials();

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

            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(jwtSettings),jwtSettings);//appsettings.json/jswsettings
            services.AddSingleton(jwtSettings);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };
            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(authOpt =>
                {
                    authOpt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authOpt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    authOpt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(configureOptions =>
                {
                    configureOptions.SaveToken = true;
                    configureOptions.TokenValidationParameters = tokenValidationParameters;
                });



            services.AddTransient<ICarOwnersRepository, CarOwnersRepository>();
            services.AddTransient<ICarsRepository, CarsRepository>();

            services.AddTransient<ICarOwnerService, CarOwnerService>();
            services.AddTransient<ICarService, CarService>();
            services.AddTransient<ICustomMapper,CustomMapper>();
            
            services.AddScoped<ICustomUserManager,CustomUserManager>();
            services.AddScoped<ICustomSignInManager,CustomSignInManager>();
            services.AddScoped<IIdentityService,IdentityService>();

            services.AddTransient<IVehicleImageRetriever, VehicleImageRetriever>();


            //services.AddSingleton<IAuthorizationMiddlewareResultHandler, MyAuthorizationMiddlewareResultHandler>(); comment to see if error exists

            services.AddSingleton<IUriService>(provider=>{
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;//http or https
                var absoluteUri = $"{request.Scheme}://{request.Host.ToUriComponent()}/";
                return new UriService(absoluteUri);
            });

            services.AddSignalR();

            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

        }
    }
}
