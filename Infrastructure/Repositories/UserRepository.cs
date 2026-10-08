using HMRC_TAX_FLOW.Domain.Users;
using HMRC_TAX_FLOW.Application.Abstractions.Persistence;
using HMRC_TAX_FLOW.Infrastructure.MongoDB;
using MongoDB.Driver;

namespace HMRC_TAX_FLOW.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly MongoDbContext _context;

    public UserRepository(MongoDbContext context) => _context = context;

    public async Task<bool> TryCreateAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);
            return true;
        }
        catch (MongoWriteException exception) when (exception.WriteError?.Code == 11000)
        {
            return false;
        }
    }

    public async Task<User?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(user => user.Username, username);
        return await _context.Users.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(user => user.Id, id);
        return await _context.Users.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(existing => existing.Id, user.Id);
        var options = new FindOneAndReplaceOptions<User>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await _context.Users.FindOneAndReplaceAsync(filter, user, options, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(user => user.Id, id);
        var result = await _context.Users.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount == 1;
    }
}
