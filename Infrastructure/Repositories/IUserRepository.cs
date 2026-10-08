using System;
using System.Threading.Tasks;
using HMRC_TAX_FLOW.Domain.Users;

namespace HMRC_TAX_FLOW.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(Guid id);
    }
}