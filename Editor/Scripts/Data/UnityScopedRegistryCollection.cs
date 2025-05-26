using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pckgs
{
    public class UnityScopedRegistryCollection : Collection<UnityScopedRegistry>
    {
        public UnityScopedRegistryCollection(IEnumerable<UnityScopedRegistry> data) : base(data)
        {
        }

        public void Save()
        {
            var manifest = ProjectPackageManifest.LoadManifest();
            manifest.ScopedRegistries = Data.ToList();
            manifest.Save();
        }
        public void Reload()
        {
            var manifest = ProjectPackageManifest.LoadManifest();
            var newData = manifest.ScopedRegistries;

            var currentUrls = new HashSet<string>(Data.Select(d => d.Url));
            var newUrls = new HashSet<string>(newData.Select(d => d.Url));

            // Removed data: items in current data but not in new data
            var removedData = Data.Where(d => !newUrls.Contains(d.Url)).ToArray();
            // Added data: items in new data but not in current data
            var addedData = newData.Where(d => !currentUrls.Contains(d.Url)).ToArray();

            foreach (var data in removedData)
                Remove(data);
            foreach (var data in addedData)
                Add(data);
        }

        public static UnityScopedRegistryCollection Load()
        {
            return new UnityScopedRegistryCollection(ProjectPackageManifest.LoadManifest().ScopedRegistries);
        }

    }
}
