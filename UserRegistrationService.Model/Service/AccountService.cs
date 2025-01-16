using System.Text.Json;
using System.Text;
using UserRegistrationService.Core.Models.ResultModels;
using UserRegistrationService.Model.Contracts.Services;
using UserRegistrationService.Model.Models.InputModels;
using UserRegistrationService.Core.Mapper;
using UserRegistrationService.Core.Models.ResponseModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;
using UserRegistrationService.Core.Models.ConfigurationModels;

namespace UserRegistrationService.Model.Service
{
    internal class AccountService : IAccountService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AccountMapper _accountMapper;
        private readonly IOptions<JwtModel> _configuration;

        public AccountService(IHttpClientFactory httpClientFactory, AccountMapper accountMapper, IOptions<JwtModel> configuration)
        {
            _httpClientFactory = httpClientFactory;
            _accountMapper = accountMapper;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<string>> RegisterAsync(RegisterInput model)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:44303"); // Set the BaseAddress

            // Serialize the model to JSON manually using System.Text.Json
            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/user/register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // Return success with the response content
                var successMessage = await response.Content.ReadAsStringAsync();
                return ServiceResponse<string>.SuccessResponse(successMessage);
            }

            // Return error response with the error message
            var errorMessage = await response.Content.ReadAsStringAsync();
            return ServiceResponse<string>.ErrorResponse(errorMessage);
        }

        public async Task<string> LoginAsync(LoginInput model)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:44303"); // Set the BaseAddress

            // Serialize the model to JSON manually using System.Text.Json
            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/user/authenticate", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response to DatabaseServiceResponse
                var databaseResponse = JsonSerializer.Deserialize<DatabaseServiceResponse>(jsonResponse);

                if (databaseResponse != null)
                {
                    // Use Mapperly to map the response
                    var user = _accountMapper.Map(databaseResponse);
                     var token = GenerateJwtToken(user);
                    return token;
                }
            }

            // Return error response with the error message
            throw new Exception("Login failed");
        }

        private string GenerateJwtToken(LoginResult user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Id.ToString()),
                new Claim("role", "user") // You can add any custom claims you need
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.PrivateKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration.Value.Issuer,
                audience: _configuration.Value.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1), // You can set the expiration time
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
