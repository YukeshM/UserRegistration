using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserRegistrationService.Core.Mapper;
using UserRegistrationService.Core.Models.ConfigurationModels;
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

            //to inject this service for controller
            services.Configure<JwtModel>(configuration.GetSection("Jwt"));

            //for getting jwt properties
            var issuer = configuration.GetSection("Jwt").GetSection("Issuer");
            var audience = configuration.GetSection("Jwt").GetSection("Audience");
            var privateKey = configuration.GetSection("Jwt").GetSection("PrivateKey");

            //jwt token
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer.Value,
                    ValidAudience = audience.Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey.Value))
                };
            });


            services.AddScoped<AccountMapper>();

            services.AddHttpClient();

            return services;
        }
    }
}
