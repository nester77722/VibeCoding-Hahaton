using MediatR;

namespace ChatApp.Application.Commands.Groups;

public record DeleteGroupCommand(
    Guid GroupId,
    Guid RequestingUserId // User making the request (must be group creator)
) : IRequest<bool>;