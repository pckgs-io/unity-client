using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Tomlyn;
using Tomlyn.Model;
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
            var manifest = UnityPackageManifest.LoadManifest();
            manifest.ScopedRegistries = Data.ToList();
            manifest.Save();
        }

        public static UnityScopedRegistryCollection Load()
        {
            return new UnityScopedRegistryCollection(UnityPackageManifest.LoadManifest().ScopedRegistries);
        }


    }
}
