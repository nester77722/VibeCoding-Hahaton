using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ChatApp.Application.Commands.Groups;

public record CreateGroupCommand(string Name, Guid CreatorId) : IRequest<Group>;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(x => x.CreatorId)
            .NotEmpty();
    }
}

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, Group>
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public CreateGroupCommandHandler(IUserRepository userRepository, IGroupRepository groupRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
    }

    public async Task<Group> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var creator = await _userRepository.GetByIdAsync(request.CreatorId, cancellationToken);
        if (creator == null)
            throw new InvalidOperationException("Creator not found");

        var group = new Group(request.Name, creator);
        await _groupRepository.AddAsync(group, cancellationToken);

        return group;
    }
}