using backend.Model.Users;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Database;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
            .HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
        modelBuilder.Entity<User>() 
            .HasKey(x => x.Id);
        modelBuilder.Entity<RefreshToken>()
            .HasKey(x => x.Token);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}