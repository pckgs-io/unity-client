using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Pckgs
{
    public class ProjectPackageCollection : Collection<ProjectPackage>
    {
        public ProjectPackageCollection(IEnumerable<ProjectPackage> data) : base(data)
        {
        }

        public static ProjectPackageCollection Load()
        {
            string[] guids = AssetDatabase.FindAssets("package t:TextAsset", new[] { "Assets" });

            var list = new List<ProjectPackage>();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith("package.json"))
                {
                    var metadata = default(PackageMetadata);
                    try
                    {
                        var metadataJson = File.ReadAllText(path);
                        metadata = JsonConvert.DeserializeObject<PackageMetadata>(metadataJson);
                    }
                    catch
                    {
                        continue;
                    }

                    list.Add(new ProjectPackage
                    {
                        Path = path,
                        Guid = guid,
                        Metadata = metadata
                    });
                }
            }

            return new ProjectPackageCollection(list);
        }

    }
}
