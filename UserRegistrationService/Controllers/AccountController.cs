using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserRegistrationService.Core.Models.ResultModels;
using UserRegistrationService.Model.Contracts.Services;
using UserRegistrationService.Model.Models.InputModels;

namespace UserRegistrationService.Controllers
{

    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var response = await _accountService.RegisterAsync(model);

        if (response.Success)
        {
            return Ok(response); // Return success response with data
        }
        else
        {
            return BadRequest(response); // Return error response with message
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var response = await _accountService.LoginAsync(model);

        if (response.Success)
        {
                var token = GenerateJwtToken(response);
            return Ok(response); // Return success response with data
        }
        else
        {
            return Unauthorized(response); // Return error response with message
        }
    }


        private string GenerateJwtToken(LoginResultModel user)
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
