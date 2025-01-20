using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserRegistrationService.Core.Contracts.Services;
using UserRegistrationService.Core.Models.InputModels;
using UserRegistrationService.Core.Models.ResponseModels;
using UserRegistrationService.Core.Models.ResultModels;

namespace UserRegistrationService.Api.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController(IAccountService accountService) : ControllerBase
    {

        /// <summary>
        /// Registers a new user. 
        /// On success, returns a success message; on failure, returns an appropriate error message.
        /// </summary>
        /// <param name="registerModel">The model containing user registration details.</param>
        /// <returns>A response indicating success or failure of the registration process.</returns>
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromForm] RegisterInput registerModel)
        {
            var response = await accountService.RegisterAsync(registerModel);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Authenticates the user and generates a JWT token on successful login. 
        /// On failure, returns an appropriate error message.
        /// </summary>
        /// <param name="loginModel">The model containing user login details (email and password).</param>
        /// <returns>A response with the JWT token on success or an error message on failure.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<LoginResponse>>> Login([FromBody] LoginInput loginModel)
        {
            var response = await accountService.LoginAsync(loginModel);

            return ServiceResponse<LoginResponse>.SuccessResponse(response, "Log in success");
        }


        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <returns>A list of all users.</returns>

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public async Task<List<UserResponse>> GetAllUsersAsync()
        {
            return await accountService.GetUsers();
        }


        [HttpGet("TestApi")]
        public ActionResult TestApi()
        {
            return Ok("User registration service Api request success");
        }

    }
}
