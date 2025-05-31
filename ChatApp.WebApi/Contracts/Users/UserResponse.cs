namespace ChatApp.WebApi.Contracts.Users;

public record UserResponse(
    Guid Id,
    string Username,
    string Name
);