using ChatApp.Application.Interfaces;
using MediatR;

namespace ChatApp.Application.Queries.Groups;

public class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery, GroupDto?>
{
    private readonly IGroupRepository _groupRepository;

    public GetGroupByIdQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<GroupDto?> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.GroupId);
        if (group == null)
        {
            return null;
        }

        // Verify that the requesting user is a member of the group
        if (!group.Members.Any(m => m.Id == request.RequestingUserId))
        {
            throw new InvalidOperationException("User is not a member of this group");
        }

        return new GroupDto(
            group.Id,
            group.Name,
            group.CreatorId,
            group.Creator.Name,
            group.CreatedAt,
            group.Members.Select(m => new GroupMemberDto(
                m.Id,
                m.Username,
                m.Name
            )).ToList()
        );
    }
}