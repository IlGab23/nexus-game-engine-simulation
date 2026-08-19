using FluentValidation;
using MediatR;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.RefreshTokenFeat.Commands;

public record LoginWithRefreshTokenCommand(string OldRefreshToken) : IRequest<Result<LoginWithRefreshTokenCommandOutput>>;
public record LoginWithRefreshTokenCommandOutput(string NewJTW, string NewRefreshToken);

public sealed class LoginWithRefreshTokenCommandValidator : AbstractValidator<LoginWithRefreshTokenCommand>
{
    public LoginWithRefreshTokenCommandValidator()
    {
        RuleFor(x => x.OldRefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")

            // 64 byte convertiti in Base64 sono sempre lunghi 88 caratteri.
            // Questo previene attacchi DoS in cui un utente malintenzionato 
            // invia una stringa di 10MB per farla hashare al tuo server bloccando la CPU.

            .Length(88).WithMessage("Refresh token must be exactly 88 characters long.")

            // Assicura che la stringa contenga solo caratteri validi per il formato Base64.

            .Matches(@"^[a-zA-Z0-9\+/]+={0,2}$").WithMessage("Refresh token format is invalid.");
    }
}