using DatabaseService.Core.Contracts.Services;
using DatabaseService.Core.Models.InputModels;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseService.Controllers
{

    [ApiController]
    [Route("api/user")]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInput model)
        {
            var response = await userService.Register(model);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginInput model)
        {
            var response = await userService.Authenticate(model);
            return Ok(response);
        }

        [HttpPost("userAlreadyRegister")]
        public async Task<IActionResult> UserAlreadyRegister([FromBody] ExistingRegisterInput model)
        {
            var response = await userService.UserAlreadyRegistered(model);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
