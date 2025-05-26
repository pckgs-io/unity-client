namespace Pckgs
{
    public class PckgsApi
    {
        public static string EndPoint => Routes.Backend;

        public UnityPackagesApi Packages { get; private set; }
        public AccessTokenApi AccessTokens { get; private set; }
        public FilesApi FilesApi { get; private set; }

        public PckgsApi()
        {
            var token = PckgsWindow.AccessToken;
            Packages = new UnityPackagesApi(EndPoint + "/packages", token);
            AccessTokens = new AccessTokenApi(EndPoint + "/access-tokens", token);
            FilesApi = new FilesApi(EndPoint + "/files", token);
        }
    }
}
