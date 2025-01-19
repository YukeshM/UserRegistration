using DatabaseService.Core.Contracts.Services;
using DatabaseService.Core.DataAccess;
using DatabaseService.Core.DataAccess.IdentityModel;
using DatabaseService.Core.Mapper;
using DatabaseService.Core.Models.InputModels;
using DatabaseService.Core.Models.ResultModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

namespace DatabaseService.Core.Services
{
    public class UserService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        UserMapper userMapper,
        UserManagementDbContext userManagementDbContext) : IUserService
    {
        public async Task<ServiceResponse<string>> Register(RegisterInput model)
        {
            // Check if email already exists
            var existingUserByEmail = await userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                return ServiceResponse<string>.ErrorResponse("User with this email already exists.");
            }

            // Check if username already exists
            var existingUserByUsername = await userManager.FindByNameAsync(model.Username);
            if (existingUserByUsername != null)
            {
                return ServiceResponse<string>.ErrorResponse("User with this username already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                LastName = model.LastName,
                RegistrationDate = model.RegistrationDate
            };

            var userDocument = userMapper.MapUserDocument(model);

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded && user != null)
            {
                userDocument.UserId = user.Id;
                await userManagementDbContext.Documents.AddAsync(userDocument);
                await userManagementDbContext.SaveChangesAsync();
                return ServiceResponse<string>.SuccessResponse(null, "User registered successfully");
            }

            return ServiceResponse<string>.ErrorResponse("Registration failed", result.Errors.FirstOrDefault()?.Description);
        }

        public async Task<LoginResult> Authenticate(LoginInput model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    return userMapper.Map(user);
                }

                throw new AuthenticationException();
            }

            throw new InvalidCredentialException("Invalid Credentials");
        }

        public async Task<ServiceResponse<string>> UserAlreadyRegistered(ExistingRegisterInput model)
        {
            var existingEmail = await userManager.FindByEmailAsync(model.Email);
            var existingUserName = await userManager.FindByNameAsync(model.Username);

            if (existingEmail != null  && existingEmail.Email == model.Email)
            {
                return ServiceResponse<string>.ErrorResponse("User with this email already exists.");
            }

            if (existingUserName != null && existingUserName.UserName == model.Username)
            {
                return ServiceResponse<string>.ErrorResponse("User with this username already exists.");
            }


            return ServiceResponse<string>.SuccessResponse(null, "User with these details does not exist.");
        }
    }
}
