using MediatR;

namespace ChatApp.Application.Commands.Groups;

public record RemoveGroupMemberCommand(
    Guid GroupId,
    Guid RequestingUserId, // User making the request (must be group creator)
    Guid MemberIdToRemove
) : IRequest<bool>;