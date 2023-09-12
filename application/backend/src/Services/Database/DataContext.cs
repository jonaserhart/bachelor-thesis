using backend.Extensions;
using backend.Model.Analysis;
using backend.Model.Analysis.Expressions;
using backend.Model.Analysis.Graphical;
using backend.Model.Analysis.KPIs;
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
            .HasValue<CountExpression>(backend.Model.Enum.ExpressionType.Count)
            .HasValue<CountIfMultipleExpression>(backend.Model.Enum.ExpressionType.CountIfMultiple)
            .HasValue<SumIfMultipleExpression>(backend.Model.Enum.ExpressionType.SumIfMultiple);

        modelBuilder.Entity<DoIfMultipleExpression>(dme =>
        {
            dme.HasMany(x => x.Conditions)
                .WithOne()
                .HasForeignKey(x => x.ExpressionId);

            dme.Navigation(x => x.Conditions).AutoInclude();
        });

        modelBuilder.Entity<Report>(report =>
        {
            report
                .Property(x => x.KPIsAndValues)
                .HasJsonConversion();

            report
                .Property(x => x.QueryResults)
                .HasJsonConversion();

            report
                .Property(x => x.Created)
                .HasDefaultValueSql("EXTRACT(EPOCH FROM NOW())::BIGINT");
        });

        modelBuilder.Entity<KPI>()
            .Property(x => x.AcceptableValues)
            .HasDefaultValue("any");

        modelBuilder.Entity<MathOperationExpression>(moe =>
        {
            moe.HasOne(x => x.Left)
                .WithMany()
                .HasForeignKey(x => x.LeftId)
                .OnDelete(DeleteBehavior.SetNull);

            moe.HasOne(x => x.Right)
                .WithMany()
                .HasForeignKey(x => x.RightId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AnalysisModel>(model =>
        {
            model.HasMany(x => x.Graphical)
                .WithOne(x => x.Model)
                .HasForeignKey("ModelId")
                .OnDelete(DeleteBehavior.Cascade);

            model.HasMany(x => x.KPIs)
                .WithOne(x => x.AnalysisModel)
                .HasForeignKey(x => x.AnalysisModelId)
                .OnDelete(DeleteBehavior.Cascade);

            model.HasMany(x => x.Reports)
                .WithOne(x => x.AnalysisModel)
                .HasForeignKey(x => x.AnalysisModelId)
                .OnDelete(DeleteBehavior.Cascade);

            model.HasMany(x => x.KPIFolders)
                .WithOne(x => x.AnalysisModel)
                .HasForeignKey(x => x.ModelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<KPIFolder>(kpiFolder =>
        {
            kpiFolder
                .HasMany(x => x.SubFolders)
                .WithOne(x => x.ParentFolder)
                .HasForeignKey(x => x.ParentFolderId)
                .OnDelete(DeleteBehavior.Cascade);

            kpiFolder
                .HasMany(x => x.KPIs)
                .WithOne(x => x.Folder)
                .HasForeignKey(x => x.FolderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GraphicalConfiguration>()
            .HasMany(x => x.Items)
            .WithOne(x => x.Configuration)
            .HasForeignKey("GraphicalConfigId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GraphicalReportItem>(reportItem =>
        {
            reportItem
                .HasOne(x => x.Layout)
                .WithOne(x => x.GraphicalReportItem)
                .HasForeignKey<GraphicalReportItemLayout>(x => x.I)
                .OnDelete(DeleteBehavior.Cascade);

            reportItem
                .HasOne(x => x.DataSources)
                .WithOne(x => x.GraphicalReportItem)
                .HasForeignKey<GraphicalItemDataSources>(x => x.ItemId);

            reportItem
                .HasOne(x => x.Properties)
                .WithOne(x => x.Item)
                .HasForeignKey<GraphicalReportItemProperties>(x => x.ItemId);
        });

        modelBuilder.Entity<GraphicalReportItemProperties>(properties =>
        {
            properties.Property(x => x.ListFields).HasJsonConversion();
        });

        modelBuilder.Entity<GraphicalItemDataSources>()
            .Property(x => x.KPIs)
            .HasJsonConversion();
    }

    public async Task<T> GetByIdOrThrowAsync<T>(Guid? id) where T : class
    {
        if (id == null)
            throw new ArgumentException("id");

        var found = await FindAsync<T>(id) ?? throw new DbKeyNotFoundException(id, typeof(T));
        return found;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserModel> UserModels { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AnalysisModel> AnalysisModels { get; set; }

    // KPIs
    public DbSet<KPI> KPIs { get; set; }
    public DbSet<KPIFolder> KPIFolders { get; set; }
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
    public DbSet<CountIfExpression> CountIfExpressions { get; set; }
    public DbSet<DoIfMultipleExpression> DoIfMultipleExpressions { get; set; }
    public DbSet<CountIfMultipleExpression> CountIfMultipleExpressions { get; set; }
    public DbSet<SumIfMultipleExpression> SumIfMultipleExpressions { get; set; }
    public DbSet<Condition> ExpressionConditions { get; set; }

    // Reports
    public DbSet<Report> Reports { get; set; }

    // config
    public DbSet<GraphicalConfiguration> GraphicalConfigurations { get; set; }
    public DbSet<GraphicalReportItem> GraphicalReportItems { get; set; }
    public DbSet<GraphicalItemDataSources> GraphicalItemDataSources { get; set; }
}