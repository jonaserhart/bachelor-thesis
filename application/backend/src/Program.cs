using System.IdentityModel.Tokens.Jwt;
using backend.Middleware;
using backend.Model.Config;
using backend.Services.Database;
using backend.Services.OAuth;
using backend.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using backend.Services.DevOps;
using Newtonsoft.Json.Converters;
using backend.Services.Expressions;
using backend.Services.DevOps.Custom;
using backend.Services.DevOps.Custom.API;
using Microsoft.AspNetCore.Authorization;
using backend.Services.Security.Handlers;
using backend.Services.Security;
using backend.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(opts =>
{
    opts.SerializerSettings.Converters.Add(new StringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile(Path.Join(builder.Environment.ContentRootPath, "src", "appsettings.Development.json"), optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile(Path.Join(builder.Environment.ContentRootPath, "appsettings.json"), optional: false, reloadOnChange: true);
}

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

builder.Services.Configure<AuthenticationConfig>(builder.Configuration.GetSection("Auth"));
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
        ValidIssuer = builder.Configuration.GetSection("Auth:OAuth:JWTIssuer").Value,
        ValidAudience = builder.Configuration.GetSection("Auth:OAuth:JWTAudience").Value,
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

builder.Services.AddScoped<IAuthorizationHandler, ModelAuthorizationHandler>();
builder.Services.AddScoped<ISecurityService, SecurityService>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<DataContext>()
    .AddCheck<HealthCheck>("Main");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<DataContext>();
    if (context != null && context.Database.CanConnect())
    {
        context.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("DEVELOPMENT");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors(cp => cp.SetIsOriginAllowed(host => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
}
app.UseHttpsRedirection();

// Middleware
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = (context, report) =>
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var o = JObject.FromObject(new
        {
            status = new
            {
                code = report.Status.ToString(),
                details = report.Entries.Select(x => new { key = x.Key, value = x.Value }).ToArray()
            }
        }
        );

        return context.Response.WriteAsync(o.ToString());
    }
});

app.Run();
