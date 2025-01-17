using DatabaseService.Core.Contracts.Services;
using DatabaseService.Core.DataAccess;
using DatabaseService.Core.DataAccess.IdentityMapper;
using DatabaseService.Core.Mapper;
using DatabaseService.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseService.Core
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services
            services.AddScoped<IUserService, UserService>();

            services.AddDbContext<UserManagementDbContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Configure other services (e.g., EF Core, Identity)
            services.AddDbContext<CustomIdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Add Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<CustomIdentityDbContext>()
                .AddDefaultTokenProviders();

            // Add other services here
            // services.AddHttpClient(), services.AddScoped<MyService>(), etc.

            services.AddScoped<UserMapper>();

            return services;
        }
    }
}
