using HMRC_TAX_FLOW.Application.Authentication;
using HMRC_TAX_FLOW.Application.Users;
using HMRC_TAX_FLOW.Domain.Users;
using HMRC_TAX_FLOW.Infrastructure.Authentication;
using HMRC_TAX_FLOW.Infrastructure.Repositories;
using HMRC_TAX_FLOW.Infrastructure.MongoDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace HMRC_TAX_FLOW.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            BsonSerializer.RegisterSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(GuidRepresentation.Standard));

            services.AddOptions<MongoDbSettings>()
                .Bind(configuration.GetSection("MongoDb"))
                .Validate(
                    settings => !string.IsNullOrWhiteSpace(settings.ConnectionString),
                    "MongoDb:ConnectionString must be configured.")
                .Validate(
                    settings => !string.IsNullOrWhiteSpace(settings.DatabaseName),
                    "MongoDb:DatabaseName must be configured.")
                .ValidateOnStart();

            services.AddSingleton<MongoDbContext>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IJwtService, JwtService>();
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            return services;
        }
    }
}