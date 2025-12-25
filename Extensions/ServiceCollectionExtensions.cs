using Microsoft.Extensions.DependencyInjection;
using Personal.Services;
using Personal.Services.Interfaces;

namespace Personal.Extensions
{
    /// <summary>
    /// Servis kayıtları için extension metodlar
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Uygulama servislerini DI container'a kaydeder
        /// </summary>
        /// <param name="services">Servis koleksiyonu</param>
        /// <returns>Servis koleksiyonu</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Core services
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IMarkdownService, MarkdownService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ICultureService, CultureService>();
            services.AddScoped<IAdminService, AdminService>();

            // Backward compatibility - concrete types can still be injected
            services.AddScoped<ProjectService>();
            services.AddScoped<MarkdownService>();
            services.AddScoped<LocalizationService>();
            services.AddScoped<CultureService>();
            services.AddScoped<AdminService>();

            return services;
        }
    }
}
