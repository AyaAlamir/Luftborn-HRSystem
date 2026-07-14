using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;
using HRSystem.Application.Auth;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Interfaces.Base;
using HRSystem.Infrastructure.Security;
using Microsoft.Extensions.Configuration;

namespace HRSystem.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseRepository<AppUser> _users;
        private readonly PasswordService _passwordService;
        private readonly TokenService _tokenService;
        private readonly string _googleClientId;

        public AuthService(
            IBaseRepository<AppUser> users,
            PasswordService passwordService,
            TokenService tokenService,
            IConfiguration configuration)
        {
            _users = users;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _googleClientId = configuration["Authentication:Google:ClientId"];
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            await EnsureDefaultUserAsync();

            var users = await _users.GetAllAsync();
            var user = users.FirstOrDefault(item =>
                string.Equals(item.UserName, request.UserName, StringComparison.OrdinalIgnoreCase));

            if (user == null || !_passwordService.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            return CreateResponse(user);
        }

        public async Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(_googleClientId))
            {
                return null;
            }

            var payload = await GoogleJsonWebSignature.ValidateAsync(
                request.IdToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleClientId }
                });

            var users = await _users.GetAllAsync();
            var user = users.FirstOrDefault(item =>
                string.Equals(item.Email, payload.Email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                user = await _users.AddAsync(new AppUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    AuthProvider = "Google"
                });
            }

            return CreateResponse(user);
        }

        private async Task EnsureDefaultUserAsync()
        {
            var users = await _users.GetAllAsync();
            if (users.Any(item => string.Equals(item.UserName, "admin", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            await _users.AddAsync(new AppUser
            {
                UserName = "admin",
                Email = "admin@hrsystem.local",
                PasswordHash = _passwordService.Hash("Admin@123"),
                AuthProvider = "Local"
            });
        }

        private AuthResponse CreateResponse(AppUser user)
        {
            return new AuthResponse
            {
                AccessToken = _tokenService.Create(user),
                UserName = user.UserName,
                Email = user.Email
            };
        }
    }
}
