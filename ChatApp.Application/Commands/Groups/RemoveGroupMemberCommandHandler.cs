using ChatApp.Application.Interfaces;
using MediatR;

namespace ChatApp.Application.Commands.Groups;

public class RemoveGroupMemberCommandHandler : IRequestHandler<RemoveGroupMemberCommand, bool>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public RemoveGroupMemberCommandHandler(
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(RemoveGroupMemberCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.GroupId);
        if (group == null)
        {
            throw new InvalidOperationException("Group not found");
        }

        if (group.CreatorId != request.RequestingUserId)
        {
            throw new InvalidOperationException("Only group creator can remove members");
        }

        if (request.MemberIdToRemove == group.CreatorId)
        {
            throw new InvalidOperationException("Cannot remove group creator");
        }

        var memberToRemove = await _userRepository.GetByIdAsync(request.MemberIdToRemove);
        if (memberToRemove == null)
        {
            throw new InvalidOperationException("User not found");
        }

        if (!group.Members.Any(m => m.Id == request.MemberIdToRemove))
        {
            throw new InvalidOperationException("User is not a member of the group");
        }

        group.RemoveMember(memberToRemove);
        await _groupRepository.UpdateAsync(group);

        return true;
    }
}