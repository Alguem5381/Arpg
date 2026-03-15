using Arpg.Application.Auth;
using Microsoft.EntityFrameworkCore;
using Arpg.Core.Models;
using Arpg.Core.Models.Customer;
using Arpg.Infrastructure.Auth;

namespace Arpg.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Sheet> Sheets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Template> Templates { get; set; }
    public DbSet<Code> Codes { get; set; }
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}