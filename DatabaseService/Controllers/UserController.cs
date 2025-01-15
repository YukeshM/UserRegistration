using DatabaseService.Core.Contracts.Services;
using DatabaseService.Core.Models.InputModels;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseService.Controllers
{

    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var response = await _userService.Register(model);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginModel model)
        {
            var response = await _userService.Authenticate(model);

            if (response.Success)
            {
                return Ok(response);
            }

            return Unauthorized(response);
        }
    }
}
