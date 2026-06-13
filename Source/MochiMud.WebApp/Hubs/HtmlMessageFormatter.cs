using System.Net;

namespace MochiMud.WebApp.Hubs
{
    internal static class HtmlMessageFormatter
    {
        public static string FormatTextMessage(string message)
        {
            return $"<div class=\"message message-text\">{HtmlEncode(message)}</div>";
        }

        public static string FormatBanner(string banner)
        {
            return $"<pre class=\"message message-banner\">{HtmlEncode(banner)}</pre>";
        }

        public static string HtmlEncode(string value)
        {
            return WebUtility.HtmlEncode(value);
        }
    }
}
