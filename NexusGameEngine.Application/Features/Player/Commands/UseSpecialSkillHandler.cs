using MediatR;
using NexusGameEngine.Application.Interfaces;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public class UseSpecialSkillHandler(IApplicationDbContext appDbContext, TimeProvider timeProvider) : IRequestHandler<UseSpecialSkillCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(UseSpecialSkillCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

}
