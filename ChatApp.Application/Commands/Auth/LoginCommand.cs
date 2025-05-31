using MediatR;

namespace ChatApp.Application.Commands.Auth;

public record LoginCommand(string Username, string Password) : IRequest<LoginResponse>;

public record LoginResponse(string Token, string Username);