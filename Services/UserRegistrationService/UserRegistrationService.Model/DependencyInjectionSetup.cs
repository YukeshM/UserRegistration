using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserRegistrationService.Core.Contracts.Services;
using UserRegistrationService.Core.Mapper;
using UserRegistrationService.Core.Models.ConfigurationModels;
using UserRegistrationService.Core.Service;

namespace UserRegistrationService.Core
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services
            services.AddScoped<IAccountService, AccountService>();

            // Register FluentValidation
            //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            //services.AddFluentValidationAutoValidation()
            //        .AddFluentValidationClientsideAdapters();

            //services.AddValidatorsFromAssemblyContaining<RegisterModelValidator>();
            //services.AddValidatorsFromAssemblyContaining<LoginModelValidator>();


            //services.AddControllers()
            //    .AddFluentValidation(fv =>
            //    {
            //        fv.RegisterValidatorsFromAssemblyContaining<RegisterModelValidator>();
            //        fv.RegisterValidatorsFromAssemblyContaining<LoginModelValidator>();
            //    });

            //to inject this service for controller
            services.Configure<JwtModel>(configuration.GetSection("Jwt"));
            services.Configure<ConfigurationModel>(configuration.GetSection("Configuration"));

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


            services.AddCors(options =>
            {
                options.AddPolicy("Allowhost", builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                    //builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            services.AddAuthorization();

            services.AddControllers();

            services.AddScoped<AccountMapper>();

            services.AddHttpClient();

            return services;
        }
    }
}
