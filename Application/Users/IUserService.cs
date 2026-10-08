using System;
using System.Threading.Tasks;
using HMRC_TAX_FLOW.Domain.Users;

namespace HMRC_TAX_FLOW.Application.Users
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(Guid id);
    }
}