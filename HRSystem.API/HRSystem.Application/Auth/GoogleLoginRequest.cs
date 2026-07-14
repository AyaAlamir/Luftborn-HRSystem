using System.ComponentModel.DataAnnotations;

namespace HRSystem.Application.Auth
{
    public class GoogleLoginRequest
    {
        [Required]
        public string IdToken { get; set; }
    }
}
