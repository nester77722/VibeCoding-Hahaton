namespace ChatApp.WebApi.Contracts.Auth;

public record LoginResponse(
    string Token,
    string Username
);