using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Pckgs
{
    public class UnityPackageCollection : Collection<UnityPackage>
    {
        public UnityPackageCollection(IEnumerable<UnityPackage> data) : base(data)
        {
        }

        public static UnityPackageCollection Load()
        {
            string[] guids = AssetDatabase.FindAssets("package t:TextAsset", new[] { "Assets" });

            var list = new List<UnityPackage>();
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

                    list.Add(new UnityPackage
                    {
                        Path = path,
                        Guid = guid,
                        Metadata = metadata
                    });
                }
            }

            return new UnityPackageCollection(list);
        }

    }
}
