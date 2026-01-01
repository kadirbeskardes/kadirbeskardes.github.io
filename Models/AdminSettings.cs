using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace Personal.Models
{
    /// <summary>
    /// Admin config dosyas?ndan okunan ayarlar
    /// </summary>
    public class AdminConfig
    {
        [JsonPropertyName("adminPassword")]
        public string AdminPassword { get; set; } = "admin123";

        [JsonPropertyName("sessionTimeoutMinutes")]
        public int SessionTimeoutMinutes { get; set; } = 60;

        [JsonPropertyName("maxLoginAttempts")]
        public int MaxLoginAttempts { get; set; } = 5;

        [JsonPropertyName("lockoutMinutes")]
        public int LockoutMinutes { get; set; } = 15;
    }

    public static class AdminSettings
    {
        // Runtime'da config dosyas?ndan yüklenen ayarlar
        private static AdminConfig? _config;
        
        // Config yüklenene kadar kullan?lacak varsay?lan de?erler
        public static int SessionTimeoutMinutes => _config?.SessionTimeoutMinutes ?? 60;
        public static int MaxLoginAttempts => _config?.MaxLoginAttempts ?? 5;
        public static int LockoutMinutes => _config?.LockoutMinutes ?? 15;

        // Storage keys
        public const string AdminStorageKey = "admin_session";
        public const string ProjectsStorageKey = "admin_projects";
        public const string LoginAttemptsKey = "admin_login_attempts";

        /// <summary>
        /// Config'i yükler
        /// </summary>
        public static void LoadConfig(AdminConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Config yüklü mü kontrolü
        /// </summary>
        public static bool IsConfigLoaded => _config != null;

        /// <summary>
        /// ?ifre do?rulamas? yapar
        /// </summary>
        public static bool VerifyPassword(string password)
        {
            if (_config == null)
                return false;

            // Düz metin kar??la?t?rma
            if (password == _config.AdminPassword)
                return true;

            // Hash kar??la?t?rma (e?er config'de hash saklan?yorsa)
            var inputHash = HashPassword(password);
            return string.Equals(inputHash, _config.AdminPassword, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// ?ifreyi SHA256 ile hash'ler
        /// </summary>
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        /// <summary>
        /// Güvenli rastgele session token olu?turur
        /// </summary>
        public static string GenerateSessionToken()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

    /// <summary>
    /// Oturum bilgilerini saklamak için model
    /// </summary>
    public class AdminSession
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(Token) && DateTime.UtcNow < ExpiresAt;
    }

    /// <summary>
    /// Giri? denemelerini takip etmek için model
    /// </summary>
    public class LoginAttemptInfo
    {
        public int AttemptCount { get; set; }
        public DateTime? LockoutUntil { get; set; }
        public DateTime LastAttempt { get; set; }

        public bool IsLockedOut => LockoutUntil.HasValue && DateTime.UtcNow < LockoutUntil.Value;
        public int RemainingAttempts => Math.Max(0, AdminSettings.MaxLoginAttempts - AttemptCount);
    }
}
