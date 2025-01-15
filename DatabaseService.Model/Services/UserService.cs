using DatabaseService.Core.Contracts.Services;
using DatabaseService.Core.DataAccess.IdentityMapper;
using DatabaseService.Core.Models.InputModels;
using DatabaseService.Core.Models.ResultModels;
using DatabaseService.Model.Model;
using Microsoft.AspNetCore.Identity;

//using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DatabaseService.Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtModel> configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ServiceResponse<string>> Register(RegisterModel model)
        {
            // Check if email already exists
            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                return ServiceResponse<string>.ErrorResponse("User with this email already exists.");
            }

            // Check if username already exists
            var existingUserByUsername = await _userManager.FindByNameAsync(model.Username);
            if (existingUserByUsername != null)
            {
                return ServiceResponse<string>.ErrorResponse("User with this username already exists.");
            }

            // Create user
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                LastName = model.LastName,
                RegistrationDate = model.RegistrationDate
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return ServiceResponse<string>.SuccessResponse(null, "User registered successfully");
            }

            return ServiceResponse<string>.ErrorResponse("Registration failed", result.Errors.FirstOrDefault()?.Description);
        }

        public async Task<ServiceResponse<object>> Authenticate(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    //var token = GenerateJwtToken(user);
                    return ServiceResponse<object>.SuccessResponse(new
                    {
                        //token,
                        username = user.UserName,
                        id = user.Id,
                        email = user.Email,
                    }, "Authentication successful");
                }

                return ServiceResponse<object>.ErrorResponse("Invalid credentials");
            }

            return ServiceResponse<object>.ErrorResponse("Invalid credentials");
        }
    }
}
