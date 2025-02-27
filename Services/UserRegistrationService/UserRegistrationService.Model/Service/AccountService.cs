﻿using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using UserRegistrationService.Core.Contracts.Services;
using UserRegistrationService.Core.Mapper;
using UserRegistrationService.Core.Models.ConfigurationModels;
using UserRegistrationService.Core.Models.InputModels;
using UserRegistrationService.Core.Models.ResponseModels;
using UserRegistrationService.Core.Models.ResultModels;

namespace UserRegistrationService.Core.Service
{
    public class AccountService(IHttpClientFactory httpClientFactory, AccountMapper accountMapper, IOptions<JwtModel> jwtConfiguration, IOptions<ConfigurationModel> appConfiguration) : IAccountService
    {
        public async Task<ServiceResponse<string>> RegisterAsync(RegisterInput model)
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(appConfiguration.Value.DBUrl);

            var existingRegisterModel = accountMapper.MapExistingRegister(model);

            var jsonContentForExistingRegister = new StringContent(JsonSerializer.Serialize(existingRegisterModel), Encoding.UTF8, "application/json");

            var responseForExistingRegister = await client.PostAsync("/api/user/userAlreadyRegister", jsonContentForExistingRegister);

            if (responseForExistingRegister.IsSuccessStatusCode)
            {
                // var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                var folderPath = appConfiguration.Value.FolderPath;

                try
                {
                    // Ensure the folder exists
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    //log, remo
                    throw new ArgumentException("The specified drive does not exist.");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("An error occurred while creating the directory.", ex);
                }

                // Validate the document
                if (model.Document == null || model.Document.Length == 0)
                {
                    throw new ArgumentException("Document is required and cannot be empty.");
                }

                // Validate the file name (check for invalid characters)
                if (string.IsNullOrWhiteSpace(model.Document.FileName) ||
                    model.Document.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    throw new ArgumentException("Invalid file name.");
                }

                // Validate the folder path
                if (string.IsNullOrWhiteSpace(folderPath) || folderPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                {
                    throw new ArgumentException("Invalid folder path.");
                }

                var filePath = Path.Combine(folderPath, model.Document.FileName);

                // Validate file path
                if (string.IsNullOrWhiteSpace(filePath) || filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                {
                    throw new ArgumentException("Invalid file path.");
                }

                var inputModelForDb = accountMapper.Map(model);

                if (model.Document.Length > 0)
                {
                    inputModelForDb.FileName = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
                    inputModelForDb.OriginalFileName = model.Document.FileName;

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Document.CopyTo(stream);
                    }
                }



                var jsonContent = new StringContent(JsonSerializer.Serialize(inputModelForDb), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/user/register", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var successMessage = await response.Content.ReadAsStringAsync();
                    return ServiceResponse<string>.SuccessResponse(successMessage);
                }
                var error = await response.Content.ReadAsStringAsync();
                return ServiceResponse<string>.ErrorResponse(error);
            }
            var errorMessage = await responseForExistingRegister.Content.ReadAsStringAsync();
            return ServiceResponse<string>.ErrorResponse(errorMessage);
        }

        public async Task<LoginResponse> LoginAsync(LoginInput model)
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(appConfiguration.Value.DBUrl);

            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/user/authenticate", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var databaseResponse = JsonSerializer.Deserialize<DatabaseServiceResponse>(jsonResponse);

                if (databaseResponse != null)
                {
                    var user = accountMapper.Map(databaseResponse);
                    var token = GenerateJwtToken(user);
                    return new LoginResponse { Token = token };
                }
            }
            var errorJsonResponse = await response.Content.ReadAsStringAsync();

            var errorDatabaseResponse = JsonSerializer.Deserialize<ServiceResponse<string>>(errorJsonResponse);

            throw new InvalidCredentialException(errorDatabaseResponse.Message);
        }


        public async Task<List<UserResponse>> GetUsers()
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(appConfiguration.Value.DBUrl);

            var response = await client.GetAsync("/api/user/GetAllUsers");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var databaseResponse = JsonSerializer.Deserialize<List<UserListInput>>(jsonResponse);

                if (databaseResponse != null)
                {
                    var user = accountMapper.MapUser(databaseResponse);
                    return user;
                }
            }
            var errorJsonResponse = await response.Content.ReadAsStringAsync();

            var errorDatabaseResponse = JsonSerializer.Deserialize<ServiceResponse<string>>(errorJsonResponse);

            throw new InvalidOperationException(errorDatabaseResponse.Message);
        }

        private string GenerateJwtToken(LoginResult user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Id.ToString()),
                new Claim("role", user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Value.PrivateKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtConfiguration.Value.Issuer,
                audience: jwtConfiguration.Value.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
