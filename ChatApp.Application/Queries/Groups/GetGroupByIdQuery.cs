using MediatR;

namespace ChatApp.Application.Queries.Groups;

public record GetGroupByIdQuery(
    Guid GroupId,
    Guid RequestingUserId // To verify the user has access to the group
) : IRequest<GroupDto?>;