using HRSystem.Application.Auth;
using HRSystem.Application.Localization;
using HRSystem.Domain.Interfaces.Base;
using HRSystem.Infrastructure.Localization;
using HRSystem.Infrastructure.Persistence;
using HRSystem.Infrastructure.Security;
using HRSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HRSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddSingleton<PasswordService>();
            services.AddSingleton<TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ILocalizationService, LocalizationService>();

            return services;
        }
    }
}
