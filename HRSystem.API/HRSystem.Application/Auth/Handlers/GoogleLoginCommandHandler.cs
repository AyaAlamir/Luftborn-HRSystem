using System.Threading;
using System.Threading.Tasks;
using HRSystem.Application.Auth.Commands;
using MediatR;

namespace HRSystem.Application.Auth.Handlers
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, AuthResponse>
    {
        private readonly IAuthService _authService;

        public GoogleLoginCommandHandler(IAuthService authService) { _authService = authService; }

        public Task<AuthResponse> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            return _authService.LoginWithGoogleAsync(request.Request);
        }
    }
}
