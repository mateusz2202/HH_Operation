using BlazorApp.Application.Features.Brands;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorApp.Application.Validators.Features.Brands;

public class AddEditBrandCommandValidator : AbstractValidator<AddEditBrandCommand>
{
    public AddEditBrandCommandValidator(IStringLocalizer<AddEditBrandCommandValidator> localizer)
    {
        RuleFor(request => request.Name)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(localizer["Name is required!"]);
        RuleFor(request => request.Description)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(localizer["Description is required!"]);
        RuleFor(request => request.Tax)
            .GreaterThan(0).WithMessage(localizer["Tax must be greater than 0"]);
    }
}