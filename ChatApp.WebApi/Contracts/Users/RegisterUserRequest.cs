namespace ChatApp.WebApi.Contracts.Users;

public record RegisterUserRequest(
    string Username,
    string Name,
    string Password
);