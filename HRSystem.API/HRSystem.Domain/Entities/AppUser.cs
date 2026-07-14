using HRSystem.Domain.Entities.Base;

namespace HRSystem.Domain.Entities
{
    public class AppUser : BaseEntity
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string AuthProvider { get; set; }
    }
}
