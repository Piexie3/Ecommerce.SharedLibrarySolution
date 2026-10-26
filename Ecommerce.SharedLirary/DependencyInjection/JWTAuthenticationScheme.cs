using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Ecommerce.SharedLirary.DependencyInjection
{
    public static class JWTAuthenticationScheme
    {
        public static IServiceCollection AddJWTAuthenticationScheme(this IServiceCollection service, IConfiguration config)
        {
            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
                    var issuer = config.GetSection("Authentication:Issuer").Value!;
                    var audience = config.GetSection("Authentication:Audience").Value!;
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        // TODO() - modify/configure refresh token and change this to true
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                    };
                });
            return service;
        }
    }
}
