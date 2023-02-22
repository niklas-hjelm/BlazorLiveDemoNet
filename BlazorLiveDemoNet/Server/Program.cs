using BlazorLiveDemoNet.DataAccess.Contexts;
using BlazorLiveDemoNet.DataAccess.Models;
using BlazorLiveDemoNet.DataAccess.Repositories;
using BlazorLiveDemoNet.Server.Hubs;
using BlazorLiveDemoNet.Server.Services;
using BlazorLiveDemoNet.Server.Services.Interfaces;
using BlazorLiveDemoNet.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRazorPages();

builder.Services.AddDbContext<UserContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("UserDb");
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ChatRepository>();

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapHub<ChatHub>("/hubs/chat");

app.MapGet("/allMessages", async (ChatRepository repo) => await repo.GetAllMessages());

app.MapPost("/user/register", async (IAuthService authService, UserRegisterDto dto) =>
{
    var user = new UserModel() {Email = dto.Email};
    return await authService.RegisterUserAsync(user, dto.Password);
});

app.MapPost("/user/login", async (IAuthService authService, UserRegisterDto dto) =>
{
    return await authService.LoginUserAsync(dto.Email, dto.Password);
});


app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.Run();
