using Microsoft.AspNetCore.Mvc;
using UserRegistrationService.Model.Model;

namespace UserRegistrationService.Controllers
{

    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Create client with a named client and set BaseAddress
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:44303");  // Set the BaseAddress

            // Now you can use relative URI
            var response = await client.PostAsJsonAsync("/api/user/register", model);

            if (response.IsSuccessStatusCode)
            {
                // Read the success message from the response content
                var successMessage = await response.Content.ReadAsStringAsync();
                return Ok(successMessage); // Return the success message
            }
            else
            {
                // Read the error message from the response content
                var errorMessage = await response.Content.ReadAsStringAsync();
                return BadRequest(errorMessage); // Return the error message
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Create client with BaseAddress set
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:44303");  // Set the BaseAddress

            // Now you can use relative URI
            var response = await client.PostAsJsonAsync("/api/user/authenticate", model);

            if (response.IsSuccessStatusCode)
            {
                // Read and return the success message from the response content
                var successMessage = await response.Content.ReadAsStringAsync();
                return Ok(successMessage);
            }

            // Read and return the error message if the authentication fails
            var errorMessage = await response.Content.ReadAsStringAsync();
            return Unauthorized(errorMessage);  // Return the error message as Unauthorized

        }
    }
}
