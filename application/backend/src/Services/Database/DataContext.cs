using backend.Extensions;
using backend.Model.Analysis;
using backend.Model.Analysis.Expressions;
using backend.Model.Exceptions;
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

        modelBuilder.Entity<MathOperationExpression>()
            .HasOne(x => x.Left)
            .WithOne()
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<MathOperationExpression>()
            .HasOne(x => x.Right)
            .WithOne()
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<Expression>()
            .HasDiscriminator(x => x.Type)
            .HasValue<AddExpression>(backend.Model.Enum.ExpressionType.Add)
            .HasValue<AvgExpression>(backend.Model.Enum.ExpressionType.Avg)
            .HasValue<DivExpression>(backend.Model.Enum.ExpressionType.Div)
            .HasValue<MaxExpression>(backend.Model.Enum.ExpressionType.Max)
            .HasValue<MinExpression>(backend.Model.Enum.ExpressionType.Min)
            .HasValue<MultiplyExpression>(backend.Model.Enum.ExpressionType.Multiply)
            .HasValue<SubtractExpression>(backend.Model.Enum.ExpressionType.Subtract)
            .HasValue<SumExpression>(backend.Model.Enum.ExpressionType.Sum)
            .HasValue<NumericValueExpression>(backend.Model.Enum.ExpressionType.Value)
            .HasValue<CountIfExpression>(backend.Model.Enum.ExpressionType.CountIf)
            .HasValue<PlainQueryExpression>(backend.Model.Enum.ExpressionType.Plain)
            .HasValue<CountExpression>(backend.Model.Enum.ExpressionType.Count);

        modelBuilder.Entity<Report>()
            .Property(x => x.KPIsAndValues)
            .HasJsonConversion();

        modelBuilder.Entity<Report>()
            .Property(x => x.QueryResults)
            .HasJsonConversion();

        modelBuilder.Entity<Report>()
            .Property(x => x.Created)
            .HasDefaultValueSql("EXTRACT(EPOCH FROM NOW())::BIGINT");

        modelBuilder.Entity<KPI>()
            .Property(x => x.AcceptableValues)
            .HasDefaultValue("any");

    }

    public async Task<T> GetByIdOrThrowAsync<T>(Guid id) where T : class
    {
        var found = await this.FindAsync<T>(id);

        if (found == null)
            throw new DbKeyNotFoundException(id, typeof(T));

        return found;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserModel> UserModels { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AnalysisModel> AnalysisModels { get; set; }

    // KPIs
    public DbSet<KPI> KPIs { get; set; }
    public DbSet<Expression> Expressions { get; set; }
    public DbSet<AddExpression> AddExpression { get; set; }
    public DbSet<AvgExpression> AvgExpression { get; set; }
    public DbSet<DivExpression> DivExpression { get; set; }
    public DbSet<MaxExpression> MaxExpression { get; set; }
    public DbSet<MinExpression> MinExpression { get; set; }
    public DbSet<MultiplyExpression> MultiplyExpression { get; set; }
    public DbSet<SubtractExpression> SubtractExpression { get; set; }
    public DbSet<SumExpression> SumExpression { get; set; }
    public DbSet<NumericValueExpression> NumericValueExpression { get; set; }
    public DbSet<PlainQueryExpression> PlainQueryExpressions { get; set; }

    // Reports
    public DbSet<Report> Reports { get; set; }
}