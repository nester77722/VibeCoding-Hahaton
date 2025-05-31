using ChatApp.Application.Interfaces;
using MediatR;

namespace ChatApp.Application.Commands.Groups;

public class AddGroupMemberCommandHandler : IRequestHandler<AddGroupMemberCommand, bool>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public AddGroupMemberCommandHandler(
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(AddGroupMemberCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.GroupId);
        if (group == null)
        {
            throw new InvalidOperationException("Group not found");
        }

        if (group.CreatorId != request.RequestingUserId)
        {
            throw new InvalidOperationException("Only group creator can add members");
        }

        var newMember = await _userRepository.GetByIdAsync(request.NewMemberId);
        if (newMember == null)
        {
            throw new InvalidOperationException("User not found");
        }

        if (group.Members.Any(m => m.Id == request.NewMemberId))
        {
            throw new InvalidOperationException("User is already a member of the group");
        }

        group.AddMember(newMember);
        await _groupRepository.UpdateAsync(group);

        return true;
    }
}