using BankingApi;
using BankingApi.Abstractions.Interfaces;
using BankingApi.Helpers;
using BankingApi.Middleware;
using BankingApi.PolicyHandlers;
using BankingApi.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestaurantBooking.BusinesApi.Data;
using Serilog;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File(LoggerHelper.GetLogFilePath(), rollingInterval: RollingInterval.Day)
    .CreateLogger();

var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
var connectionString = builder.Configuration.GetConnectionString(nameof(BankingAppDbContext));

builder.Services.AddDbContext<BankingAppDbContext>(config =>
    {
        config.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
    });

builder.Services.AddMemoryCache();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services
    .AddAuthentication(config =>
    {
        config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        config.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, config =>
    {
        config.Authority = "https://localhost:7293";
        config.ClientId = "BankingApiClient_id";
        config.ClientSecret = "BankingApiClient_secret";
        config.SaveTokens = true;
        config.ResponseType = "code";
        config.Scope.Add("offline_access");
        config.GetClaimsFromUserInfoEndpoint = true;
        config.ClaimActions.MapJsonKey(ClaimTypes.Role, ClaimTypes.Role);
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, config =>
    {
        config.Authority = "https://localhost:7293";
        config.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ClockSkew = TimeSpan.FromSeconds(5)
        };
    });

builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("HasAdminRole", policyBuilder =>
    {
        policyBuilder.RequireClaim(ClaimTypes.Role);
        policyBuilder.RequireRole("Administrator");
        policyBuilder.AddRequirements(new HasAdminRoleRequirments("Administrator"));
    });
});

builder.Services.AddSingleton<IAuthorizationHandler, HasAdminRoleRequirmentsHandler>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Banking api", Version = "v1" });
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri("https://localhost:7293/connect/authorize"),
                    TokenUrl = new Uri("https://localhost:7293/connect/token"),
                    Scopes = new Dictionary<string, string>
                    {
                                { "openid", "OpenID Connect" },
                                { "profile", "User profile" },
                                { "BankingApi.BankingApi", "Access to BankingApi" },
                    }
                }
            }
        });
        options.OperationFilter<AuthorizeCheckOperationFilter>();

    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking api");

        c.OAuthClientId("BankingApiClient_id");
        c.OAuthClientSecret("BankingApiClient_secret");
        c.OAuthAppName("Swagger UI for Banking api");
        c.OAuthUsePkce();
        c.OAuth2RedirectUrl("https://localhost:44334/swagger/oauth2-redirect.html");
    });
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseCors("AllowAllOrigins");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//using (var scope = app.Services.CreateScope())
//{
//    DataBuilderInitializer.Init(scope.ServiceProvider);
//}
app.Run();





