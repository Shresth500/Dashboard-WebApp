using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Ascendion.Products.Dashboard.Auth;

public static class AuthExtensions
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
      services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = jwtOptions!.ValidateIssue,
                ValidateAudience = jwtOptions!.ValidateAudience,
                ValidateLifetime = jwtOptions!.ValidateLifetime,
                ValidateIssuerSigningKey = jwtOptions!.ValidateIssuerSigningKey,
                ValidIssuer = jwtOptions!.ValidIssuer,
                ValidAudience = jwtOptions!.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
            });
        return services;
    } 
}
