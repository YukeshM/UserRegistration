using System.Text.Json;
using System.Text;
using UserRegistrationService.Core.Models.ResultModels;
using UserRegistrationService.Model.Contracts.Services;
using UserRegistrationService.Model.Models.InputModels;
using UserRegistrationService.Core.Mapper;
using UserRegistrationService.Core.Models.ResponseModels;

namespace UserRegistrationService.Model.Service
{
    internal class AccountService : IAccountService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AccountMapper _accountMapper;

        public AccountService(IHttpClientFactory httpClientFactory, AccountMapper accountMapper)
        {
            _httpClientFactory = httpClientFactory;
            _accountMapper = accountMapper;
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

        public async Task<LoginResult> LoginAsync(LoginInput model)
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
                    return _accountMapper.Map(databaseResponse);
                }
            }

            // Return error response with the error message
            throw new Exception("Login failed");
        }
    }
}
