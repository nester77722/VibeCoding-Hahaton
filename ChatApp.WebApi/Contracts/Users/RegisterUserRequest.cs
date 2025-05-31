namespace ChatApp.WebApi.Contracts.Users;

public record RegisterUserRequest(
    string Username,
    string Password
);