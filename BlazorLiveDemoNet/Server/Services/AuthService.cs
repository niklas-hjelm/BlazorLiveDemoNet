using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BlazorLiveDemoNet.DataAccess.Contexts;
using BlazorLiveDemoNet.DataAccess.Models;
using BlazorLiveDemoNet.Server.Services.Interfaces;
using BlazorLiveDemoNet.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlazorLiveDemoNet.Server.Services;

public class AuthService : IAuthService
{
    private readonly UserContext _userContext;
    private readonly IConfiguration _configuration;

    public AuthService(UserContext userContext, IConfiguration configuration)
    {
        _userContext = userContext;
        _configuration = configuration;
    }

    public async Task<ServiceResponse<int>> RegisterUserAsync(UserModel user, string password)
    {
        if (await CheckUserExistsAsync(user.Email))
        {
            return new ServiceResponse<int>()
            {
                Success = false,
                Message = "User already exists"
            };
        }
        
        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        await _userContext.UserModels.AddAsync(user);
        await _userContext.SaveChangesAsync();

        return new ServiceResponse<int>()
        {
            Data = user.Id,
            Success = true,
            Message = "Great Success!"
        };
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    public async Task<ServiceResponse<string>> LoginUserAsync(string email, string password)
    {
        if (!await CheckUserExistsAsync(email))
        {
            return new ServiceResponse<string>()
            {
                Success = false,
                Message = "Wrong user name or password"
            };
        }

        var user = await _userContext.UserModels.FirstAsync(u => u.Email == email);

        if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            return new ServiceResponse<string>()
            {
                Success = false,
                Message = "Wrong user name or password"
            };
        }

        return new ServiceResponse<string>()
        {
            Data = CreateToken(user),
            Success = true,
            Message = "TJOHOOOOOOOOOOOOOO!"
        };

    }

    private string CreateToken(UserModel user)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, user.Nickname),
            new (ClaimTypes.Email, user.Email),
            new (ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var secret = _configuration.GetSection("AppSecrets:Secret").Value;

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            signingCredentials: creds,
            expires: DateTime.UtcNow.AddDays(1),
            claims: claims
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerifyPasswordHash(string password, byte[] userPasswordHash, byte[] userPasswordSalt)
    {
        using (var hmac = new HMACSHA512(userPasswordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(userPasswordHash);
        }
    }

    public async Task<bool> CheckUserExistsAsync(string email)
    {
        return await _userContext.UserModels.AnyAsync(u=> u.Email.Equals(email));
    }
}