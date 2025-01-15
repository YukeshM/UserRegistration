using System.Text.Json;
using System.Text;
using UserRegistrationService.Core.Models.ResultModels;
using UserRegistrationService.Model.Contracts.Services;
using UserRegistrationService.Model.Models.InputModels;

namespace UserRegistrationService.Model.Service
{
    internal class AccountService : IAccountService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ServiceResponse<string>> RegisterAsync(RegisterModel model)
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

        public async Task<LoginResultModel> LoginAsync(LoginModel model)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:44303"); // Set the BaseAddress

            // Serialize the model to JSON manually using System.Text.Json
            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/user/authenticate", jsonContent);

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
    }
}
