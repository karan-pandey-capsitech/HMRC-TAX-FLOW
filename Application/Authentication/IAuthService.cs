using System.Threading.Tasks;
using HMRC_TAX_FLOW.Application.Authentication.DTOs;
using HMRC_TAX_FLOW.Domain.Users;

namespace HMRC_TAX_FLOW.Application.Authentication
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<User> RegisterAsync(RegisterRequest request);
    }
}