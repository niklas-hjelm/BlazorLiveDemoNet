using BlazorLiveDemoNet.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorLiveDemoNet.DataAccess.Contexts;

public class UserContext : DbContext
{
    public DbSet<UserModel> UserModels { get; set; }

    public UserContext(DbContextOptions options) : base(options)
    {
        
    }
}