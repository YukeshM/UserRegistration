using DatabaseService.Core.Contracts.Services;
using DatabaseService.Core.DataAccess.Domain;
using DatabaseService.Core.Models.InputModels;
using DatabaseService.Core.Models.ResultModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatabaseService.Api.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController(IUserService userService) : ControllerBase
    {

        /// <summary>
        /// Registers a new user. 
        /// On success, returns a success message; on failure, returns an appropriate error message.
        /// </summary>
        /// <param name="model">The model containing user registration details.</param>
        /// <returns>A response indicating success or failure of the registration process.</returns>
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterInput model)
        {
            var response = await userService.Register(model);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }


        /// <summary>
        /// Authenticates the user and generates a JWT token on successful login. 
        /// On failure, returns an appropriate error message.
        /// </summary>
        /// <param name="model">The model containing user login details (email and password).</param>
        /// <returns>A response with the JWT token on success or an error message on failure.</returns>
        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate([FromBody] LoginInput model)
        {
            var response = await userService.Authenticate(model);
            return Ok(response);
        }


        /// <summary>
        /// Check whether the user is already register and it will return reponse.
        /// On failure, returns an appropriate error message.
        /// </summary>
        /// <param name="model">The model containing username and email.</param>
        /// <returns>A response with true on success or an error message on failure.</returns>

        [HttpPost("userAlreadyRegister")]
        public async Task<ActionResult> UserAlreadyRegister([FromBody] ExistingRegisterInput model)
        {
            var response = await userService.UserAlreadyRegistered(model);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }


        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <returns>A list of all users.</returns>
        
        [HttpGet("GetAllUsers")]
        public async Task<List<UserResponse>> GetAllUsersAsync()
        {
            return await userService.GetAllUsersAsync();
        }


        [HttpGet("TestApi")]
        public ActionResult TestApi()
        {
            return Ok("Database service Api request success");
        }
    }
}
