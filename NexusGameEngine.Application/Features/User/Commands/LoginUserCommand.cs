using FluentValidation;
using MediatR;
using NexusGameEngine.Domain.Entities.ValueObjects;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.User.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<Result<LoginCommandOutput>>;
public record LoginCommandOutput(string JwtToken, string RefreshToken);

public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters.");

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");
    }
}
