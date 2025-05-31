using MediatR;

namespace ChatApp.Application.Commands.Groups;

public record AddGroupMemberCommand(
    Guid GroupId,
    Guid RequestingUserId, // User making the request (must be group admin)
    Guid NewMemberId
) : IRequest<bool>;