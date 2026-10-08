using System.Text;
using HMRC_TAX_FLOW.Application;
using HMRC_TAX_FLOW.Infrastructure;
using HMRC_TAX_FLOW.Infrastructure.MongoDB;
using HMRC_TAX_FLOW.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();

var key = builder.Configuration["Jwt:Key"];
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];
var expiresMinutes = builder.Configuration.GetValue<int?>("Jwt:ExpiresMinutes");

if (string.IsNullOrWhiteSpace(key) || Encoding.UTF8.GetByteCount(key) < 32)
{
    throw new InvalidOperationException("Jwt:Key must be configured with at least 32 UTF-8 bytes.");
}

if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
{
    throw new InvalidOperationException("Jwt:Issuer and Jwt:Audience must be configured.");
}

if (expiresMinutes is < 1 or > 1440)
{
    throw new InvalidOperationException("Jwt:ExpiresMinutes must be between 1 and 1440.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        RequireExpirationTime = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

await app.Services.GetRequiredService<MongoDbContext>().EnsureIndexesAsync();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
