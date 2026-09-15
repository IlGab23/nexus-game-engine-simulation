using MediatR;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Features.Player.Commands;

public record UseSpecialSkillCommand(Guid PlayerId) : IRequest<Result<bool>>;