using DatabaseService.Core.Models.InputModels;
using DatabaseService.Core.Models.ResultModels;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseService.Core.Contracts.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<string>> Register(RegisterModel model);
        Task<ServiceResponse<object>> Authenticate(LoginModel model);
    }
}
