using MediatR;

namespace ChatApp.Application.Queries.Groups;

public record GetUserGroupsQuery(Guid UserId) : IRequest<List<GroupDto>>;

public record GroupMemberDto(
    Guid Id,
    string Username,
    string Name
);

public record GroupDto(
    Guid Id,
    string Name,
    Guid CreatorId,
    string CreatorName,
    DateTime CreatedAt,
    List<GroupMemberDto> Members
);