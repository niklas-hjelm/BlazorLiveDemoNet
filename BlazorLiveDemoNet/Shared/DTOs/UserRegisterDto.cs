using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlazorLiveDemoNet.Shared.DTOs;

public class UserRegisterDto
{
    [Required, StringLength(10, MinimumLength = 3)]
    public string Nickname { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }

    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}