namespace ChatApp.WebApi.Contracts.Auth;

public record LoginRequest(
    string Username,
    string Password
);