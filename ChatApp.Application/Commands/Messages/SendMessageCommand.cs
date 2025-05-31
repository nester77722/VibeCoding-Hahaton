using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ChatApp.Application.Commands.Messages;

public record SendMessageCommand(
    string Content,
    Guid SenderId,
    Guid? RecipientUserId,
    Guid? RecipientGroupId
) : IRequest<MessageDto>;

public record MessageDto(
    Guid Id,
    string Content,
    Guid SenderId,
    string SenderUsername,
    string SenderName,
    DateTime SentAt,
    Guid? RecipientUserId,
    Guid? RecipientGroupId
);

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.SenderId)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.RecipientUserId.HasValue ^ x.RecipientGroupId.HasValue)
            .WithMessage("Message must be sent either to a user or a group, but not both");
    }
}

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IMessageRepository _messageRepository;

    public SendMessageCommandHandler(
        IUserRepository userRepository,
        IGroupRepository groupRepository,
        IMessageRepository messageRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _messageRepository = messageRepository;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var sender = await _userRepository.GetByIdAsync(request.SenderId, cancellationToken);
        if (sender == null)
            throw new InvalidOperationException("Sender not found");

        Message message;

        if (request.RecipientUserId.HasValue)
        {
            var recipient = await _userRepository.GetByIdAsync(request.RecipientUserId.Value, cancellationToken);
            if (recipient == null)
                throw new InvalidOperationException("Recipient not found");

            message = new Message(request.Content, sender, recipient);
        }
        else
        {
            var group = await _groupRepository.GetByIdAsync(request.RecipientGroupId!.Value, cancellationToken);
            if (group == null)
                throw new InvalidOperationException("Group not found");

            message = new Message(request.Content, sender, recipientGroup: group);
        }

        await _messageRepository.AddAsync(message, cancellationToken);
        return new MessageDto(
            message.Id,
            message.Content,
            message.Sender.Id,
            message.Sender.Username,
            message.Sender.Name,
            message.SentAt,
            message.RecipientUserId,
            message.RecipientGroupId
        );
    }
}