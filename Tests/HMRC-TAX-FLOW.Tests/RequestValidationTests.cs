using System.ComponentModel.DataAnnotations;
using HMRC_TAX_FLOW.Application.Authentication.DTOs;
using HMRC_TAX_FLOW.Application.SA100.DTOs;
using HMRC_TAX_FLOW.Application.Users.DTOs;
using Xunit;

namespace HMRC_TAX_FLOW.Tests;

public sealed class RequestValidationTests
{
    [Fact]
    public void RegisterRequest_RejectsShortPasswordsAndInvalidEmail()
    {
        var request = new RegisterRequest
        {
            Username = "alice",
            Email = "not-an-email",
            Password = "short"
        };

        var validationResults = Validate(request);

        Assert.Contains(validationResults, result => result.MemberNames.Contains(nameof(RegisterRequest.Email)));
        Assert.Contains(validationResults, result => result.MemberNames.Contains(nameof(RegisterRequest.Password)));
    }
    [Fact]
    public void UpdateProfileRequest_AllowsOmittedFields()
    {
        var validationResults = Validate(new UpdateProfileRequest());

        Assert.Empty(validationResults);
    }

    [Fact]
    public void UpdateProfileRequest_RejectsMalformedEmail()
    {
        var validationResults = Validate(new UpdateProfileRequest { Email = "invalid" });

        Assert.Contains(validationResults, result => result.MemberNames.Contains(nameof(UpdateProfileRequest.Email)));
    }

    [Theory]
    [InlineData("2025-26", true)]
    [InlineData("2025-25", false)]
    [InlineData("2025-6", false)]
    public void CreateSa100Request_ValidatesConsecutiveTaxYear(string taxYear, bool isValid)
    {
        var results = Validate(new CreateSa100Request
        {
            ClientId = Guid.NewGuid(),
            TaxYear = taxYear
        });

        Assert.Equal(isValid, !results.Any(result =>
            result.MemberNames.Contains(nameof(CreateSa100Request.TaxYear))));
    }

    [Theory]
    [InlineData("Practice", true)]
    [InlineData("Debitam", true)]
    [InlineData("Admin", false)]
    [InlineData("Unknown", false)]
    public void AssignUserRoleRequest_OnlyAllowsAssignableRoles(string role, bool isValid)
    {
        var results = Validate(new AssignUserRoleRequest { Role = role });

        Assert.Equal(isValid, !results.Any(result =>
            result.MemberNames.Contains(nameof(AssignUserRoleRequest.Role))));
    }

    private static List<ValidationResult> Validate(object value)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(
            value,
            new ValidationContext(value),
            results,
            validateAllProperties: true);
        return results;
    }
}
