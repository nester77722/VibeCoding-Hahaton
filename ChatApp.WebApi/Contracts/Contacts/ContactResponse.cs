namespace ChatApp.WebApi.Contracts.Contacts;

public record ContactResponse(
    Guid Id,
    string Username,
    DateTime AddedAt
);