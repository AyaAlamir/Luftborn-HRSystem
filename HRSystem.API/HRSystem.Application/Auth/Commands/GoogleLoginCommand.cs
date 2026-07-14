using MediatR;

namespace HRSystem.Application.Auth.Commands
{
    public class GoogleLoginCommand : IRequest<AuthResponse>
    {
        public GoogleLoginRequest Request { get; }
        public GoogleLoginCommand(GoogleLoginRequest request) { Request = request; }
    }
}
