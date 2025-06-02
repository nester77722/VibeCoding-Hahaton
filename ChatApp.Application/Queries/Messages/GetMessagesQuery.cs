using ChatApp.Application.Common;
using MediatR;

namespace ChatApp.Application.Queries.Messages;

public record GetMessagesQuery(
    Guid UserId,
    Guid? RecipientUserId,
    Guid? RecipientGroupId,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedResult<MessageDto>>;

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