using HMRC_TAX_FLOW.Application.Abstractions.Persistence;
using HMRC_TAX_FLOW.Application.Abstractions.Security;
using HMRC_TAX_FLOW.Domain.Users;
using HMRC_TAX_FLOW.Infrastructure.Authentication;
using HMRC_TAX_FLOW.Infrastructure.MongoDB;
using HMRC_TAX_FLOW.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace HMRC_TAX_FLOW.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services.AddOptions<MongoDbSettings>()
            .Bind(configuration.GetSection("MongoDb"))
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.ConnectionString),
                "MongoDb:ConnectionString must be configured.")
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.DatabaseName),
                "MongoDb:DatabaseName must be configured.")
            .ValidateOnStart();

        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ISa100Repository, Sa100Repository>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }
}
