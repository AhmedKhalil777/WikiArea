using FluentValidation;

namespace WikiArea.Application.Features.Auth.Commands;

public class SigninCommandValidator : AbstractValidator<SigninCommand>
{
    public SigninCommandValidator()
    {
        RuleFor(x => x.UsernameOrEmail)
            .NotEmpty().WithMessage("Username or email is required")
            .MaximumLength(100).WithMessage("Username or email cannot exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters");
    }
} 