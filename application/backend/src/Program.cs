using System.IdentityModel.Tokens.Jwt;
using backend.Middleware;
using backend.Model.Config;
using backend.Services.Database;
using backend.Services.OAuth;
using backend.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using backend.Services.DevOps;
using Newtonsoft.Json.Converters;
using backend.Services.Expressions;
using backend.Services.DevOps.Custom;
using backend.Services.DevOps.Custom.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(opts =>
{
    opts.SerializerSettings.Converters.Add(new StringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddEnvironmentVariables(prefix: "SCRUM_BACKEND_");

foreach (var c in builder.Configuration.AsEnumerable())
{
    Console.WriteLine(c.Key + " = " + c.Value);
}

builder.Services
    .AddDbContext<DataContext>(opts =>
    {
        opts.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDatabase"));
    });

builder.Services.Configure<OAuthConfig>(builder.Configuration.GetSection("OAuth"));
builder.Services.Configure<DevOpsConfig>(builder.Configuration.GetSection("DevOps"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IOAuthService, MicrosoftOAuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAnalysisModelService, AnalysisModelService>();
builder.Services.AddScoped<IKPIService, KPIService>();

// Custom services
builder.Services.AddScoped<IApiClientFactory, ApiClientFactory>();
builder.Services.AddScoped<IDevOpsProviderService, AzureDevOpsProviderService>();

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("OAuth:ClientSecret").Value ?? "");

builder.Services.AddAuthentication((x) =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer((x) =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration.GetSection("OAuth:JWTIssuer").Value,
        ValidAudience = builder.Configuration.GetSection("OAuth:JWTAudience").Value,
        ValidateIssuer = true,
        ValidateAudience = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = false,
        SignatureValidator = delegate (string token, TokenValidationParameters parameters)
        {
            var jwt = new JwtSecurityToken(token);
            return jwt;
        },
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("DEVELOPMENT");
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(cp => cp.WithOrigins("http://localhost:3050").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
}
app.UseHttpsRedirection();

// Middleware
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
