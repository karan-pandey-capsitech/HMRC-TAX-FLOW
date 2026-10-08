using HMRC_TAX_FLOW.Application.SA100;
using HMRC_TAX_FLOW.Application.SA100.DTOs;
using HMRC_TAX_FLOW.Domain.Clients;
using HMRC_TAX_FLOW.Domain.SA100;
using HMRC_TAX_FLOW.Infrastructure.Repositories;
using Xunit;

namespace HMRC_TAX_FLOW.Tests;

public sealed class Sa100ServiceTests
{
    [Fact]
    public async Task CreateAsync_CreatesDraftForAssignedPracticeUser()
    {
        var client = new Client
        {
            PracticeUserId = Guid.NewGuid(),
            Name = "Example Client",
            NationalInsuranceNumber = "AB123456C"
        };
        var repository = new FakeSa100Repository();
        var service = CreateService(client, repository);

        var response = await service.CreateAsync(
            new CreateSa100Request
            {
                ClientId = client.Id,
                TaxYear = "2025-26",
                EmploymentIncome = 25000,
                EstimatedTax = 1200
            },
            client.PracticeUserId);

        Assert.Equal(Sa100Status.Draft, response.Status);
        Assert.Equal(client.Id, response.ClientId);
        Assert.Equal(client.PracticeUserId, repository.Returns.Single().PracticeUserId);
        Assert.Equal("AB123456C", response.NationalInsuranceNumber);
    }

    [Fact]
    public async Task CreateAsync_RejectsClientAssignedToAnotherPracticeUser()
    {
        var client = new Client { PracticeUserId = Guid.NewGuid() };
        var service = CreateService(client, new FakeSa100Repository());

        await Assert.ThrowsAsync<ClientAccessDeniedException>(() =>
            service.CreateAsync(
                new CreateSa100Request { ClientId = client.Id },
                Guid.NewGuid()));
    }

    [Fact]
    public async Task SubmitAsync_SubmitsDraftAndPreventsSecondSubmission()
    {
        var client = new Client { PracticeUserId = Guid.NewGuid() };
        var repository = new FakeSa100Repository();
        var service = CreateService(client, repository);
        var draft = new Sa100Return
        {
            ClientId = client.Id,
            PracticeUserId = client.PracticeUserId,
            CreatedBy = client.PracticeUserId
        };
        repository.Returns.Add(draft);

        var submitted = await service.SubmitAsync(draft.Id, client.PracticeUserId);

        Assert.Equal(Sa100Status.Submitted, submitted.Status);
        Assert.Equal(client.PracticeUserId, submitted.SubmittedBy);
        Assert.NotNull(submitted.SubmittedAt);
        await Assert.ThrowsAsync<Sa100ConflictException>(() =>
            service.SubmitAsync(draft.Id, client.PracticeUserId));
    }

    [Fact]
    public async Task GetByIdAsync_HidesReturnsFromOtherPracticeUsers()
    {
        var client = new Client { PracticeUserId = Guid.NewGuid() };
        var repository = new FakeSa100Repository();
        var taxReturn = new Sa100Return
        {
            ClientId = client.Id,
            PracticeUserId = client.PracticeUserId
        };
        repository.Returns.Add(taxReturn);
        var service = CreateService(client, repository);

        await Assert.ThrowsAsync<ClientAccessDeniedException>(() =>
            service.GetByIdAsync(taxReturn.Id, Guid.NewGuid(), isAdmin: false));
    }

    private static Sa100Service CreateService(Client client, FakeSa100Repository repository) =>
        new(repository, new FakeClientRepository(client));

    private sealed class FakeClientRepository(Client client) : IClientRepository
    {
        public Task CreateAsync(Client value, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Client?>(id == client.Id ? client : null);

        public Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Client>>([client]);

        public Task<IReadOnlyList<Client>> GetByAssignedUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Client>>(client.PracticeUserId == userId ? [client] : []);
    }

    private sealed class FakeSa100Repository : ISa100Repository
    {
        public List<Sa100Return> Returns { get; } = [];

        public Task CreateAsync(Sa100Return taxReturn, CancellationToken cancellationToken = default)
        {
            Returns.Add(taxReturn);
            return Task.CompletedTask;
        }

        public Task<Sa100Return?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Returns.FirstOrDefault(taxReturn => taxReturn.Id == id) is { } taxReturn
                ? Clone(taxReturn)
                : null);

        public Task<IReadOnlyList<Sa100Return>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Sa100Return>>(Returns.ToArray());

        public Task<IReadOnlyList<Sa100Return>> GetByPracticeUserAsync(
            Guid practiceUserId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Sa100Return>>(
                Returns.Where(taxReturn => taxReturn.PracticeUserId == practiceUserId).ToArray());

        public Task<Sa100Return?> TryUpdateDraftAsync(
            Sa100Return taxReturn,
            CancellationToken cancellationToken = default)
        {
            var index = Returns.FindIndex(
                item => item.Id == taxReturn.Id && item.Status == Sa100Status.Draft);
            if (index < 0)
            {
                return Task.FromResult<Sa100Return?>(null);
            }

            Returns[index] = Clone(taxReturn);
            return Task.FromResult<Sa100Return?>(Clone(taxReturn));
        }

        public Task<long> CountByStatusAsync(
            Sa100Status status,
            Guid? practiceUserId = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult((long)Returns.Count(taxReturn =>
                taxReturn.Status == status &&
                (!practiceUserId.HasValue || taxReturn.PracticeUserId == practiceUserId.Value)));

        private static Sa100Return Clone(Sa100Return value) => new()
        {
            Id = value.Id, ClientId = value.ClientId, PracticeUserId = value.PracticeUserId,
            TaxYear = value.TaxYear, ClientName = value.ClientName,
            NationalInsuranceNumber = value.NationalInsuranceNumber,
            EmploymentIncome = value.EmploymentIncome, SelfEmploymentIncome = value.SelfEmploymentIncome,
            OtherIncome = value.OtherIncome, TaxAlreadyPaid = value.TaxAlreadyPaid,
            EstimatedTax = value.EstimatedTax, Status = value.Status, CreatedBy = value.CreatedBy,
            CreatedAt = value.CreatedAt, UpdatedAt = value.UpdatedAt,
            SubmittedBy = value.SubmittedBy, SubmittedAt = value.SubmittedAt
        };
    }
}
