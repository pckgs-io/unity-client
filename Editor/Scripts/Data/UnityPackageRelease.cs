using System;

namespace Pckgs
{
    public class UnityPackageRelease
    {
        public UnityPackageVersion Version { get; set; }
        public bool IsPrerelease { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public PackageMetadata Metadata { get; set; }
        public FileData Tarball { get; set; }
    }
}