using System.Threading.Tasks;

namespace HRSystem.Application.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request);
    }
}
