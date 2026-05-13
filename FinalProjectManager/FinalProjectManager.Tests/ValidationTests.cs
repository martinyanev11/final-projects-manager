using FinalProjectManager.Data.Models;
using FinalProjectManager.Web.ViewModels;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace FinalProjectManager.Tests;

public class ValidationTests
{
    private IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void SupervisorRegisterViewModel_WithValidData_ReturnsNoErrors()
    {
        var model = new SupervisorRegisterViewModel
        {
            FullName = "Иван Иванов",
            Email = "ivan@test.bg",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var results = ValidateModel(model);
        Assert.Empty(results);
    }

    [Fact]
    public void SupervisorRegisterViewModel_WithInvalidEmail_ReturnsError()
    {
        var model = new SupervisorRegisterViewModel
        {
            FullName = "Иван Иванов",
            Email = "invalid-email",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var results = ValidateModel(model);
        Assert.Contains(results, r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void SupervisorRegisterViewModel_WithMismatchedPasswords_ReturnsError()
    {
        var model = new SupervisorRegisterViewModel
        {
            FullName = "Иван Иванов",
            Email = "ivan@test.bg",
            Password = "Password123!",
            ConfirmPassword = "DifferentPassword123!"
        };

        var results = ValidateModel(model);
        Assert.Contains(results, r => r.MemberNames.Contains("ConfirmPassword"));
    }
}
