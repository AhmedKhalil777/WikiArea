using FluentValidation;
using WikiArea.Core.Enums;

namespace WikiArea.Application.Features.WikiPages.Commands;

public class CreateWikiPageCommandValidator : AbstractValidator<CreateWikiPageCommand>
{
    public CreateWikiPageCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required")
            .MaximumLength(100000)
            .WithMessage("Content cannot exceed 100,000 characters");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Content type is required")
            .Must(BeValidContentType)
            .WithMessage("Invalid content type");

        RuleFor(x => x.Tags)
            .Must(tags => tags.Count <= 10)
            .WithMessage("Cannot have more than 10 tags");

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .WithMessage("Tags cannot be empty")
            .MaximumLength(50)
            .WithMessage("Tag cannot exceed 50 characters");
    }

    private static bool BeValidContentType(string contentType)
    {
        return ContentType.TryFromName(contentType, out _);
    }
} 