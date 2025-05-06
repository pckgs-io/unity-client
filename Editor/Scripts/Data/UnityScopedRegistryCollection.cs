using System.Collections.Generic;
using System.Linq;

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
            foreach (var data in Data.ToArray())
                Remove(data);
            foreach (var data in manifest.ScopedRegistries)
                Add(data);
        }

        public static UnityScopedRegistryCollection Load()
        {
            return new UnityScopedRegistryCollection(ProjectPackageManifest.LoadManifest().ScopedRegistries);
        }


    }
}
