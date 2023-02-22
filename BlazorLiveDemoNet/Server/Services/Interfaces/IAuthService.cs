using BlazorLiveDemoNet.DataAccess.Models;
using BlazorLiveDemoNet.Shared;

namespace BlazorLiveDemoNet.Server.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResponse<int>> RegisterUserAsync(UserModel user, string password);
    Task<ServiceResponse<string>> LoginUserAsync(string email, string password);
    Task<bool> CheckUserExistsAsync(string email);
}