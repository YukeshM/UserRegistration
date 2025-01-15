using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserRegistrationService.Model.Contracts.Services;
using UserRegistrationService.Model.Service;

namespace UserRegistrationService.Model
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services
            services.AddScoped<IAccountService, AccountService>();

            services.AddHttpClient();

            return services;
        }
    }
}
