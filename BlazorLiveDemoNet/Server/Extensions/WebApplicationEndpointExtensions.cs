using BlazorLiveDemoNet.DataAccess.Models;
using BlazorLiveDemoNet.Server.Services.Interfaces;
using BlazorLiveDemoNet.Shared.DTOs;

namespace BlazorLiveDemoNet.Server.Extensions;

public static class WebApplicationEndpointExtensions
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/user/register", RegisterUserHandlerAsync);
        app.MapPost("/user/login", LoginUserHandlerAsync);

        return app;
    }

    private static async Task<IResult> LoginUserHandlerAsync(IAuthService authService, UserLoginDto dto)
    {
        var response = await authService.LoginUserAsync(dto.Email, dto.Password);
        return response.Success ? Results.Ok(response) : Results.BadRequest(response);
    }

    private static async Task<IResult> RegisterUserHandlerAsync(IAuthService authService, UserRegisterDto dto)
    {
        var model = new UserModel() { Nickname = dto.Nickname, Email = dto.Email};
        var response = await authService.RegisterUserAsync(model, dto.Password);
        return response.Success ? Results.Ok(response) : Results.BadRequest(response);
    }
}