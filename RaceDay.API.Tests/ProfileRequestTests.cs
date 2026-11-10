using RaceDay.API.DTOs;
using RaceDay.API.DTOs.Participants;
using RaceDay.MVC.ViewModels;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace RaceDay.API.Tests;

public sealed class ProfileRequestTests
{
    [Fact]
    public void UpdateProfile_RequiresNamesAndAcceptsOptionalPhone()
    {
        var request = new UpdateParticipantProfileRequest
        {
            FirstName = "Alex",
            LastName = "Runner",
            PhoneNumber = "+1-555-0100"
        };

        var errors = Validate(request);

        Assert.Empty(errors);
    }

    [Fact]
    public void UpdateProfile_RejectsMissingNamesAndInvalidPhone()
    {
        var request = new UpdateParticipantProfileRequest
        {
            FirstName = "",
            LastName = "",
            PhoneNumber = "not a phone number"
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(request.FirstName)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(request.LastName)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(request.PhoneNumber)));
    }

    [Fact]
    public void Registration_RequiresMatchingPasswordsAndValidEmail()
    {
        var request = new RegisterRequest
        {
            FirstName = "Alex",
            LastName = "Runner",
            Email = "not-an-email",
            Password = "short",
            ConfirmPassword = "different"
        };

        var errors = Validate(request);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(request.Email)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(request.Password)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(request.ConfirmPassword)));
    }

    [Fact]
    public void MvcRegistration_RejectsMismatchedPasswords()
    {
        var model = new RegisterViewModel
        {
            FirstName = "Alex",
            LastName = "Runner",
            Email = "alex@example.com",
            Password = "StrongPassword!2026",
            ConfirmPassword = "DifferentPassword!2026"
        };

        var errors = Validate(model);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(model.ConfirmPassword)));
    }

    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            results,
            validateAllProperties: true);

        return results;
    }
}
