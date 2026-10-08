using HMRC_TAX_FLOW.Domain.SA100;
using HMRC_TAX_FLOW.Application.Abstractions.Persistence;
using HMRC_TAX_FLOW.Infrastructure.MongoDB;
using MongoDB.Driver;

namespace HMRC_TAX_FLOW.Infrastructure.Repositories;

public sealed class Sa100Repository(MongoDbContext context) : ISa100Repository
{
    public async Task<bool> TryCreateAsync(Sa100Return taxReturn, CancellationToken cancellationToken = default)
    {
        try
        {
            await context.Sa100Returns.InsertOneAsync(taxReturn, cancellationToken: cancellationToken);
            return true;
        }
        catch (MongoWriteException exception) when (exception.WriteError?.Code == 11000)
        {
            return false;
        }
    }

    public async Task<Sa100Return?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Sa100Returns.Find(taxReturn => taxReturn.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<Sa100Return>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Sa100Returns.Find(Builders<Sa100Return>.Filter.Empty)
            .SortByDescending(taxReturn => taxReturn.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Sa100Return>> GetByPracticeUserAsync(
        Guid practiceUserId,
        CancellationToken cancellationToken = default) =>
        await context.Sa100Returns.Find(taxReturn => taxReturn.PracticeUserId == practiceUserId)
            .SortByDescending(taxReturn => taxReturn.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<Sa100Return?> TryUpdateDraftAsync(
        Sa100Return taxReturn,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Sa100Return>.Filter.Eq(item => item.Id, taxReturn.Id) &
                     Builders<Sa100Return>.Filter.Eq(item => item.Status, Sa100Status.Draft);
        var options = new FindOneAndReplaceOptions<Sa100Return>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await context.Sa100Returns.FindOneAndReplaceAsync(
            filter,
            taxReturn,
            options,
            cancellationToken);
    }

    public async Task<long> CountByStatusAsync(
        Sa100Status status,
        Guid? practiceUserId = null,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Sa100Return>.Filter.Eq(taxReturn => taxReturn.Status, status);
        if (practiceUserId.HasValue)
        {
            filter &= Builders<Sa100Return>.Filter.Eq(
                taxReturn => taxReturn.PracticeUserId,
                practiceUserId.Value);
        }

        return await context.Sa100Returns.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}
