using backend.Model.Analysis;
using backend.Model.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);
        modelBuilder.Entity<User>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<RefreshToken>()
            .HasKey(x => x.Token);

        modelBuilder.Entity<UserModel>()
            .HasKey(x => new { x.UserId, x.ModelId });
        modelBuilder.Entity<UserModel>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserModels)
            .HasForeignKey(x => x.UserId);
        modelBuilder.Entity<UserModel>()
            .HasOne(x => x.Model)
            .WithMany(x => x.ModelUsers)
            .HasForeignKey(x => x.ModelId);

        var permissionValueComparer = new ValueComparer<IEnumerable<ModelPermission>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => (IEnumerable<ModelPermission>)c.ToHashSet());
        modelBuilder.Entity<UserModel>()
                .Property(x => x.Permissions)
                .HasConversion(
                    permissions => string.Join(',', permissions),
                    str => str.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().Select(x => Enum.Parse<ModelPermission>(x)) ?? new List<ModelPermission>())
                    .Metadata.SetValueComparer(permissionValueComparer);

        modelBuilder.Entity<Query>()
            .HasOne(x => x.Where)
            .WithOne(x => x.Query);
        modelBuilder.Entity<Query>()
            .HasMany(x => x.Select)
            .WithOne(x => x.Query)
            .HasForeignKey(x => x.QueryId);

        modelBuilder.Entity<Clause>()
            .HasMany(x => x.Clauses)
            .WithOne(x => x.ParentClause)
            .HasForeignKey(x => x.ParentClauseId);

        modelBuilder.Entity<AnalysisModel>()
            .HasOne(x => x.Project)
            .WithMany(x => x.Models)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.ClientSetNull);
        modelBuilder.Entity<AnalysisModel>()
            .HasOne(x => x.Team)
            .WithMany(x => x.Models)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.ClientSetNull);
        modelBuilder.Entity<AnalysisModel>()
            .HasMany(x => x.Queries)
            .WithOne(x => x.Model)
            .OnDelete(DeleteBehavior.ClientCascade);

    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserModel> UserModels { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AnalysisModel> AnalysisModels { get; set; }
    public DbSet<Query> Queries { get; set; }
    public DbSet<FieldInfo> FieldInfos { get; set; }
    public DbSet<Project> Projects { get; set; }
}