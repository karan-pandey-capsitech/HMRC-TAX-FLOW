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
            // Configure MongoDB BSON to handle Guid serialization
            BsonSerializer.RegisterSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(GuidRepresentation.Standard));

            // MongoDB settings
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
            services.AddSingleton<MongoDbContext>();

            // Repositories & Services
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IJwtService, JwtService>();
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            return services;
        }
    }
}