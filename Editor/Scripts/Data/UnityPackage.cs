using UnityEngine;

namespace Pckgs
{
    public class UnityPackage
    {
        public string FullPath => System.IO.Path.Combine(Application.dataPath, Path.Substring("Assets/".Length));
        public string Path { get; set; }
        public string Guid { get; set; }
        public PackageMetadata Metadata { get; set; }
    }
}
