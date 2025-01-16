using DatabaseService.Core.Contracts.Services;
using DatabaseService.Core.DataAccess.IdentityMapper;
using DatabaseService.Core.Mapper;
using DatabaseService.Core.Models.InputModels;
using DatabaseService.Core.Models.ResultModels;
using Microsoft.AspNetCore.Identity;

//using Microsoft.AspNetCore.Identity;

namespace DatabaseService.Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserMapper _userMapper;

        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            UserMapper userMapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userMapper = userMapper;
        }

        public async Task<ServiceResponse<string>> Register(RegisterInput model)
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

        public async Task<LoginResult> Authenticate(LoginInput model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    return _userMapper.Map(user);
                }

                throw new Exception();
            }

            throw new Exception();
        }
    }
}
