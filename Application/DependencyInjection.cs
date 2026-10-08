using HMRC_TAX_FLOW.Application.Authentication;
using HMRC_TAX_FLOW.Application.Clients;
using HMRC_TAX_FLOW.Application.Dashboard;
using HMRC_TAX_FLOW.Application.SA100;
using HMRC_TAX_FLOW.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace HMRC_TAX_FLOW.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<ISa100Service, Sa100Service>();
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }
}
