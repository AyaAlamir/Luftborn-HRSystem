using MediatR;

namespace HRSystem.Application.Auth.Commands
{
    public class LoginCommand : IRequest<AuthResponse>
    {
        public LoginRequest Request { get; }
        public LoginCommand(LoginRequest request) { Request = request; }
    }
}
