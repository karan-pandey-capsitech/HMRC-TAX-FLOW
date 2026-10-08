using System.ComponentModel.DataAnnotations;
using HMRC_TAX_FLOW.Application.Authentication.DTOs;
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
