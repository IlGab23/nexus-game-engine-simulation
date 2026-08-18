using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Application.Interfaces.Security;
using NexusGameEngine.Domain.Entities.ValueObjects;
using NexusGameEngine.Domain.ResultPattern;
using DomainUser = NexusGameEngine.Domain.Entities.User;

namespace NexusGameEngine.Application.Features.User.Commands;


public class RegisterUserHandler(IApplicationDbContext appDbContext, IPasswordHasher passwordHasher) : IRequestHandler<RegisterUserCommand, Result<RegistrationOutput>>
{
    public async Task<Result<RegistrationOutput>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await appDbContext.Users.AnyAsync(u => u.Email.Value == request.Email, cancellationToken)) return Error.RegistrationEmailExist;
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure) return emailResult.ErrorList;

        string hashedPassword = await passwordHasher.HashPassword(request.Password);

        var userResult = DomainUser.Create(request.UserName, emailResult.Value, hashedPassword);
        if (userResult.IsFailure) return userResult.ErrorList;

        await appDbContext.Users.AddAsync(userResult.Value, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return new RegistrationOutput(userResult.Value.Id);
    }

}
