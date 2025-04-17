namespace Pckgs
{
    public class Navigation
    {
        public static string Backend = "http://localhost:5000";
        public static string Website = "http://localhost:5173";

        public static string AccessDashboardLink => $"{Website}/access-tokens";

        public static string OrganizationWebsite(string slug) => $"{Website}/organizations/{slug}";
        public static string OrganizationRegistry(string slug) => $"{Backend}/{slug}";
    }
}