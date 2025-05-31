namespace ChatApp.WebApi.Contracts.Messages;

public record SendMessageRequest(
    string Content,
    Guid? RecipientUserId,
    Guid? RecipientGroupId
);