using ChatApp.Application.Common;
using ChatApp.Application.Interfaces;
using MediatR;

namespace ChatApp.Application.Queries.Messages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, PaginatedResult<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public GetMessagesQueryHandler(
        IMessageRepository messageRepository,
        IUserRepository userRepository,
        IGroupRepository groupRepository)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _groupRepository = groupRepository;
    }

    public async Task<PaginatedResult<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        // Validate that either recipientUserId or recipientGroupId is provided, but not both
        if (request.RecipientUserId == null && request.RecipientGroupId == null)
            throw new InvalidOperationException("Either recipientUserId or recipientGroupId must be provided");
        if (request.RecipientUserId != null && request.RecipientGroupId != null)
            throw new InvalidOperationException("Only one of recipientUserId or recipientGroupId can be provided");

        // Validate that the user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found");

        IEnumerable<Domain.Entities.Message> messages;

        if (request.RecipientUserId.HasValue)
        {
            // Validate that the recipient user exists
            var recipientUser = await _userRepository.GetByIdAsync(request.RecipientUserId.Value, cancellationToken);
            if (recipientUser == null)
                throw new InvalidOperationException("Recipient user not found");

            // Get direct messages between the two users
            messages = await _messageRepository.GetDirectMessagesAsync(request.UserId, request.RecipientUserId.Value, cancellationToken);
        }
        else
        {
            // Validate that the group exists and user is a member
            var group = await _groupRepository.GetByIdAsync(request.RecipientGroupId!.Value, cancellationToken);
            if (group == null)
                throw new InvalidOperationException("Group not found");

            if (!group.Members.Any(m => m.Id == request.UserId))
                throw new InvalidOperationException("User is not a member of this group");

            // Get group messages
            messages = await _messageRepository.GetGroupMessagesAsync(request.RecipientGroupId.Value, cancellationToken);
        }

        // Apply pagination
        var totalCount = messages.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        var paginatedMessages = messages
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        // Map to DTOs
        var messageDtos = paginatedMessages.Select(m => new MessageDto(
            m.Id,
            m.Content,
            m.SenderId,
            m.Sender.Username,
            m.Sender.Name,
            m.SentAt,
            m.RecipientUserId,
            m.RecipientGroupId
        )).ToList();

        return new PaginatedResult<MessageDto>(
            messageDtos,
            request.Page,
            request.PageSize,
            totalCount,
            totalPages
        );
    }
} 