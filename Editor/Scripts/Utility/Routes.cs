namespace Pckgs
{
    public class Routes
    {
        public static string PackagePrefix = "pckgs.io";

#if PCKGS_IO_LOCAL
        public static string Backend = "http://localhost:5000";
        public static string Website = "http://localhost:5173";
#else
        public static string Backend = "https://registry.pckgs.io";
        public static string Website = "https://pckgs.io";
#endif

        public static string AccessDashboardLink => $"{Website}/access-tokens";
        public static string UnityRegistry => $"{Backend}/upm";

        public static string AccessTokenWebsite(string slug) => $"{Website}/access-tokens/{slug}";
        public static string OrganizationWebsite(string slug) => $"{Website}/orgs/{slug}";
        public static string OrganizationRegistry(string slug) => $"{Backend}/{slug}";
        public static string PackageName(string name) => $"{PackagePrefix}.{name}";
    }
}