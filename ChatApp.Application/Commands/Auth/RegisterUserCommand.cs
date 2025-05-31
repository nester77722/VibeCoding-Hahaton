using ChatApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ChatApp.Application.Commands.Auth;

public record RegisterUserCommand(string Username, string Password) : IRequest<User>;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(100);
    }
}