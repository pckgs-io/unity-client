namespace Pckgs
{
    public record UnityPackageVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public string[] ReleaseLabels { get; set; }
        public string Metadata { get; set; }
        public string NormalizedString { get; set; }
        public string FullString { get; set; }
    }
}