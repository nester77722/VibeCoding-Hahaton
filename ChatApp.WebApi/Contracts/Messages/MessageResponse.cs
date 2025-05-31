namespace ChatApp.WebApi.Contracts.Messages;

public record MessageResponse(
    Guid Id,
    string Content,
    Guid SenderId,
    string SenderUsername,
    string SenderName,
    DateTime SentAt,
    Guid? RecipientUserId,
    Guid? RecipientGroupId
);