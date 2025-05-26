namespace Pckgs
{
    public class Routes
    {
        public static string PackagePrefix = "pckgs.io";
        public static string Backend = "http://localhost:5000";
        public static string Website = "http://localhost:5173";

        public static string AccessDashboardLink => $"{Website}/access-tokens";
        public static string UnityRegistry => $"{Backend}/upm";

        public static string AccessTokenWebsite(string slug) => $"{Website}/access-tokens/{slug}";
        public static string OrganizationWebsite(string slug) => $"{Website}/orgs/{slug}";
        public static string OrganizationRegistry(string slug) => $"{Backend}/{slug}";
        public static string PackageName(string name) => $"{PackagePrefix}.{name}";
    }
}