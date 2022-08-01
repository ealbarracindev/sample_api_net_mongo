using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using web_api.Infrastructure.Services;
using web_api.Infrastructure.Settings;

namespace web_api.Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        #region Config Context
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDatabaseSettings>(db=>db.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            return services;
        }
        #endregion
        #region Policies cors
        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration, string corsNamePolicy)
        {
            //CORS
            services.AddCors(o => o.AddPolicy(name:corsNamePolicy, builder =>
            {
                builder.AllowAnyHeader()
                       .WithMethods(configuration.GetSection("AllowedMethod").Get<string[]>())
                       .WithOrigins(configuration.GetSection("AllowedOrigin").Get<string[]>());
                       //.SetIsOriginAllowed(origin => new Uri(origin).Host == "http://localhost:4200")
            }));
            return services;
        }
        #endregion
        #region Config Dependencies injection, controller
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<UserService>();
            //services.AddTransient<IUserRepository, UserRepository>();
            return services;
        }
        #endregion
        #region Config Problem Details
        //public static IServiceCollection AddCustomProblemDetails(this IServiceCollection services) =>
        //  services
        //      .AddProblemDetails(opts =>
        //      {
        //          //configure.IncludeExceptionDetails = _ => environment.EnvironmentName == "dev";
        //          // Control when an exception is included
        //          opts.IncludeExceptionDetails = (ctx, ex) =>
        //          {
        //              // Fetch services from HttpContext.RequestServices
        //              var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
        //              return env.IsDevelopment() || env.IsStaging();
        //          };
        //      });
        #endregion
        #region Config Custom Diagnostic Handler
        //public static IServiceCollection AddCustomHostingDiagnosticHandler(this IServiceCollection services)
        //{
        //    return services.AddHostedService<HostingDiagnosticHandler>();
        //}
        #endregion
    }
}
