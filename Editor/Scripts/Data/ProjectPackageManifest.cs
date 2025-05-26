using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Pckgs
{
    public class ProjectPackageManifest
    {
        public List<UnityScopedRegistry> ScopedRegistries { get; set; } = new();
        public Dictionary<string, string> Dependencies { get; set; } = new();

        public static string PackagesFolderPath => Path.Combine(Application.dataPath.Replace("/Assets", ""), "Packages");
        public static string PackageManifestFileName => "manifest.json";
        public static string PackageManifestFilePath => Path.Combine(PackagesFolderPath, PackageManifestFileName);


        public void Save()
        {
            string manifestPath = Path.Combine(Application.dataPath.Replace("/Assets", ""), "Packages", "manifest.json");

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            var manifestJson = JsonConvert.SerializeObject(this, settings);
            File.WriteAllText(manifestPath, manifestJson);
        }
        public static ProjectPackageManifest LoadManifest()
        {
            if (!File.Exists(PackageManifestFilePath))
                return new ProjectPackageManifest();

            using var stream = new FileStream(
                    path: PackageManifestFilePath,
                    mode: FileMode.Open,
                    access: FileAccess.Read,
                    share: FileShare.ReadWrite | FileShare.Delete
                );

            using var reader = new StreamReader(stream);
            string manifestJson = reader.ReadToEnd();

            var manifest = JsonConvert.DeserializeObject<ProjectPackageManifest>(manifestJson);
            manifest.ScopedRegistries ??= new List<UnityScopedRegistry>();
            return manifest;
        }
    }
}