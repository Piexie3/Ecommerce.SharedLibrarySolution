using Ecommerce.SharedLirary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Ecommerce.SharedLirary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(
            this IServiceCollection services,
            IConfiguration config,
            string fileName
            ) where TContext : DbContext
        {
            // Add generic database context
            services.AddDbContext<TContext>(option => option
            .UseSqlServer(config.GetConnectionString("eCommerceConnection"), sqlServerOption => sqlServerOption.EnableRetryOnFailure()));

            // configure serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {message:lj} {NewLine} {Exception}",
                rollingInterval: RollingInterval.Day).CreateLogger();

            // Add JWTAuthenticationscheme
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(service: services, config: config);

            return services;

        }
        public static IApplicationBuilder UserSharedPloicies(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalException>();
            //app.UseMiddleware<ListenToOnlyApiGateway>();
            return app;
        }
    }
}
