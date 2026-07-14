using System.Threading;
using System.Threading.Tasks;
using HRSystem.Application.Auth.Commands;
using MediatR;

namespace HRSystem.Application.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService) { _authService = authService; }

        public Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return _authService.LoginAsync(request.Request);
        }
    }
}
