using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Pckgs
{
    public class ProjectPackageManifest
    {
        public List<UnityScopedRegistry> ScopedRegistries { get; set; }

        public Dictionary<string, string> Dependencies { get; set; }

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
            string manifestPath = Path.Combine(Application.dataPath.Replace("/Assets", ""), "Packages", "manifest.json");
            var manifestJson = File.ReadAllText(manifestPath);
            var manifest = JsonConvert.DeserializeObject<ProjectPackageManifest>(manifestJson);
            manifest.ScopedRegistries ??= new List<UnityScopedRegistry>();
            return manifest;
        }
    }
}