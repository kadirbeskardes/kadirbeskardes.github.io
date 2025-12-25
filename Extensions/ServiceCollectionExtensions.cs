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
            // Register concrete service implementations
            // These are registered first so they can be used by both interface and concrete type injection
            services.AddScoped<ProjectService>();
            services.AddScoped<MarkdownService>();
            services.AddScoped<LocalizationService>();
            services.AddScoped<CultureService>();
            services.AddScoped<AdminService>();

            // Register interfaces that resolve to the same scoped instances
            services.AddScoped<IProjectService>(sp => sp.GetRequiredService<ProjectService>());
            services.AddScoped<IMarkdownService>(sp => sp.GetRequiredService<MarkdownService>());
            services.AddScoped<ILocalizationService>(sp => sp.GetRequiredService<LocalizationService>());
            services.AddScoped<ICultureService>(sp => sp.GetRequiredService<CultureService>());
            services.AddScoped<IAdminService>(sp => sp.GetRequiredService<AdminService>());

            return services;
        }
    }
}
