using System;
using System.Threading.Tasks;
using HMRC_TAX_FLOW.Domain.Users;
using HMRC_TAX_FLOW.Infrastructure.MongoDB;
using MongoDB.Driver;

namespace HMRC_TAX_FLOW.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MongoDbContext _context;

        public UserRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(User user)
        {
            await _context.Users.InsertOneAsync(user);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);
            return await _context.Users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            return await _context.Users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> UpdateAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var options = new FindOneAndReplaceOptions<User> { ReturnDocument = ReturnDocument.After };
            return await _context.Users.FindOneAndReplaceAsync(filter, user, options);
        }

        public async Task DeleteAsync(Guid id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            await _context.Users.DeleteOneAsync(filter);
        }
    }
}