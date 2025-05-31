using ChatApp.Application.Interfaces;
using MediatR;

namespace ChatApp.Application.Commands.Groups;

public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, bool>
{
    private readonly IGroupRepository _groupRepository;

    public DeleteGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<bool> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.GroupId);
        if (group == null)
        {
            throw new InvalidOperationException("Group not found");
        }

        if (group.CreatorId != request.RequestingUserId)
        {
            throw new InvalidOperationException("Only group creator can delete the group");
        }

        await _groupRepository.DeleteAsync(group);
        return true;
    }
}