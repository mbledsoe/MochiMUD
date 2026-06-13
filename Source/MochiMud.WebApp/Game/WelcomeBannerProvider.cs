namespace MochiMud.WebApp.Game
{
    public class WelcomeBannerProvider
    {
        private readonly Lazy<string?> banner;

        public WelcomeBannerProvider(IHostEnvironment environment, ILogger<WelcomeBannerProvider> logger)
        {
            banner = new Lazy<string?>(() => Load(environment, logger));
        }

        public string? GetBanner()
        {
            return banner.Value;
        }

        private static string? Load(IHostEnvironment environment, ILogger logger)
        {
            var path = Path.Combine(environment.ContentRootPath, "Content", "welcome-banner.txt");

            if (!File.Exists(path))
            {
                logger.LogWarning("Welcome banner file not found at {Path}.", path);
                return null;
            }

            var contents = File.ReadAllText(path);

            return string.IsNullOrWhiteSpace(contents) ? null : contents;
        }
    }
}
