using System;
using System.Threading.Tasks;
using HMRC_TAX_FLOW.Domain.Users;
using HMRC_TAX_FLOW.Infrastructure.Repositories;

namespace HMRC_TAX_FLOW.Application.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        public UserService(IUserRepository repo) => _repo = repo;

        public Task<User?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task<User?> GetByUsernameAsync(string username) => _repo.GetByUsernameAsync(username);
        public Task<User> UpdateAsync(User user) => _repo.UpdateAsync(user);
        public Task DeleteAsync(Guid id) => _repo.DeleteAsync(id);
    }
}