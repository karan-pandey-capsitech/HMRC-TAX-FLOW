using HMRC_TAX_FLOW.Application.SA100.DTOs;
using HMRC_TAX_FLOW.Domain.Clients;
using HMRC_TAX_FLOW.Domain.SA100;
using HMRC_TAX_FLOW.Infrastructure.Repositories;

namespace HMRC_TAX_FLOW.Application.SA100;

public sealed class Sa100Service(
    ISa100Repository sa100Repository,
    IClientRepository clientRepository) : ISa100Service
{
    public async Task<IReadOnlyList<Sa100Response>> GetAllAsync(
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var returns = isAdmin
            ? await sa100Repository.GetAllAsync(cancellationToken)
            : await sa100Repository.GetByPracticeUserAsync(userId, cancellationToken);

        return returns.Select(ToResponse).ToArray();
    }

    public async Task<Sa100Response> GetByIdAsync(
        Guid id,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var taxReturn = await sa100Repository.GetByIdAsync(id, cancellationToken)
            ?? throw new Sa100NotFoundException();

        EnsureAccess(taxReturn, userId, isAdmin);
        return ToResponse(taxReturn);
    }

    public async Task<Sa100Response> CreateAsync(
        CreateSa100Request request,
        Guid practiceUserId,
        CancellationToken cancellationToken = default)
    {
        var client = await clientRepository.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new ClientNotFoundException();

        if (client.PracticeUserId != practiceUserId)
        {
            throw new ClientAccessDeniedException();
        }

        var now = DateTime.UtcNow;
        var taxReturn = new Sa100Return
        {
            ClientId = client.Id,
            PracticeUserId = practiceUserId,
            TaxYear = request.TaxYear.Trim(),
            ClientName = client.Name,
            NationalInsuranceNumber = client.NationalInsuranceNumber.Trim().ToUpperInvariant(),
            EmploymentIncome = request.EmploymentIncome,
            SelfEmploymentIncome = request.SelfEmploymentIncome,
            OtherIncome = request.OtherIncome,
            TaxAlreadyPaid = request.TaxAlreadyPaid,
            EstimatedTax = request.EstimatedTax,
            CreatedBy = practiceUserId,
            CreatedAt = now,
            UpdatedAt = now
        };

        await sa100Repository.CreateAsync(taxReturn, cancellationToken);
        return ToResponse(taxReturn);
    }

    public async Task<Sa100Response> UpdateDraftAsync(
        Guid id,
        UpdateSa100Request request,
        Guid practiceUserId,
        CancellationToken cancellationToken = default)
    {
        var taxReturn = await GetPracticeReturnAsync(id, practiceUserId, cancellationToken);
        EnsureDraft(taxReturn);

        taxReturn.EmploymentIncome = request.EmploymentIncome;
        taxReturn.SelfEmploymentIncome = request.SelfEmploymentIncome;
        taxReturn.OtherIncome = request.OtherIncome;
        taxReturn.TaxAlreadyPaid = request.TaxAlreadyPaid;
        taxReturn.EstimatedTax = request.EstimatedTax;
        taxReturn.UpdatedAt = DateTime.UtcNow;

        var updated = await sa100Repository.TryUpdateDraftAsync(taxReturn, cancellationToken)
            ?? throw new Sa100ConflictException();

        return ToResponse(updated);
    }

    public async Task<Sa100Response> SubmitAsync(
        Guid id,
        Guid practiceUserId,
        CancellationToken cancellationToken = default)
    {
        var taxReturn = await GetPracticeReturnAsync(id, practiceUserId, cancellationToken);
        EnsureDraft(taxReturn);

        taxReturn.Status = Sa100Status.Submitted;
        taxReturn.SubmittedBy = practiceUserId;
        taxReturn.SubmittedAt = DateTime.UtcNow;
        taxReturn.UpdatedAt = taxReturn.SubmittedAt.Value;

        var updated = await sa100Repository.TryUpdateDraftAsync(taxReturn, cancellationToken)
            ?? throw new Sa100ConflictException();

        return ToResponse(updated);
    }

    private async Task<Sa100Return> GetPracticeReturnAsync(
        Guid id,
        Guid practiceUserId,
        CancellationToken cancellationToken)
    {
        var taxReturn = await sa100Repository.GetByIdAsync(id, cancellationToken)
            ?? throw new Sa100NotFoundException();

        EnsureAccess(taxReturn, practiceUserId, isAdmin: false);
        return taxReturn;
    }

    private static void EnsureAccess(Sa100Return taxReturn, Guid userId, bool isAdmin)
    {
        if (!isAdmin && taxReturn.PracticeUserId != userId)
        {
            throw new ClientAccessDeniedException();
        }
    }

    private static void EnsureDraft(Sa100Return taxReturn)
    {
        if (taxReturn.Status != Sa100Status.Draft)
        {
            throw new Sa100ConflictException();
        }
    }

    private static Sa100Response ToResponse(Sa100Return taxReturn) =>
        new(
            taxReturn.Id,
            taxReturn.ClientId,
            taxReturn.TaxYear,
            taxReturn.ClientName,
            taxReturn.NationalInsuranceNumber,
            taxReturn.EmploymentIncome,
            taxReturn.SelfEmploymentIncome,
            taxReturn.OtherIncome,
            taxReturn.TaxAlreadyPaid,
            taxReturn.EstimatedTax,
            taxReturn.Status,
            taxReturn.CreatedBy,
            taxReturn.CreatedAt,
            taxReturn.UpdatedAt,
            taxReturn.SubmittedBy,
            taxReturn.SubmittedAt);
}
