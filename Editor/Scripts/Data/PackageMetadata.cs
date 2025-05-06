using System.Collections.Generic;

namespace Pckgs
{
    public class PackageMetadata
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Unity { get; set; }
        public PackageAuthor Author { get; set; }
        public string ChangeLogUrl { get; set; }
        public Dictionary<string, string> Dependencies { get; set; }
        public string DocumentationUrl { get; set; }
        public bool? HideInEditor { get; set; }
        public string[] Keywords { get; set; }
        public string License { get; set; }
        public string LicensesUrl { get; set; }
        public PackageSample[] Samples { get; set; }
        public string Type { get; set; }
        public string UnityRelease { get; set; }
    }
}
