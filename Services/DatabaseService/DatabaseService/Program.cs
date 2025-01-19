using DatabaseService.Api.Middleware;
using DatabaseService.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Logs to the console
    .WriteTo.File("logs/general-log-.txt", rollingInterval: RollingInterval.Day) // General logs
    .WriteTo.File("logs/error-log-.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, rollingInterval: RollingInterval.Day) // Error-specific logs
    .CreateLogger();

try
{
    Log.Information("Application Starting");
    var builder = WebApplication.CreateBuilder(args);


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Database service", Version = "v1" });


        // Include 'SecurityScheme' to use JWT Authentication
        var jwtSecurityScheme = new OpenApiSecurityScheme
        {
            Scheme = "bearer",
            BearerFormat = "JWT",
            Name = "JWT Authentication",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };

        c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
                jwtSecurityScheme, Array.Empty<string>() }
        });
    });


    #region customized services
    // Register dependencies
    builder.Services.AddApplicationServices(builder.Configuration);

    #endregion

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
        app.UseSwagger();
        app.UseSwaggerUI();
    //}

    app.UseMiddleware<ErrorHandlingMiddleware>();

    app.UseCors("Allowhost");

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start.");
}
finally
{
    Log.CloseAndFlush();
}
