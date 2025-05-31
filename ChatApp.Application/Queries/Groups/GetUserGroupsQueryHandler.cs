using ChatApp.Application.Interfaces;
using MediatR;

namespace ChatApp.Application.Queries.Groups;

public class GetUserGroupsQueryHandler : IRequestHandler<GetUserGroupsQuery, List<GroupDto>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GetUserGroupsQueryHandler(
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<List<GroupDto>> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var groups = await _groupRepository.GetUserGroupsAsync(request.UserId);

        return groups.Select(g => new GroupDto(
            g.Id,
            g.Name,
            g.CreatorId,
            g.Creator.Name,
            g.CreatedAt,
            g.Members.Select(m => new GroupMemberDto(
                m.Id,
                m.Username,
                m.Name
            )).ToList()
        )).ToList();
    }
}