using Ascendion.Products.Dashboard.Data;
using Microsoft.AspNetCore.Identity;

namespace Ascendion.Products.Dashboard.Auth;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<IdentityUser>()
            .AddRoles<IdentityRole>()
            .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("AscendionAPI")
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;
        });
        return services;
    }
}
