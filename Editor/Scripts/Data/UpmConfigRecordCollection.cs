using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tomlyn;
using Tomlyn.Model;
using UnityEngine;

namespace Pckgs
{
    public class UpmConfigRecordCollection : Collection<UpmConfigRecord>
    {
        public static UpmConfigRecordCollection Instance { get; private set; }
        public static string UserProfile =>
         Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".upmconfig.toml");

        static UpmConfigRecordCollection()
        {
            Instance ??= Load();
        }

        private UpmConfigRecordCollection(IEnumerable<UpmConfigRecord> data) : base(data)
        {
        }

        public void Add(string endPoint, string token, string email)
        {
            Add(new UpmConfigRecord
            {
                EndPoint = endPoint,
                Token = token,
                Email = email,
                AlwaysAuth = true
            });
        }

        public void Reload()
        {
            var collection = Load().Data;
            foreach (var d in Data.ToArray())
                Remove(d);
            foreach (var d in collection)
                Add(d);
        }

        private static UpmConfigRecordCollection Load()
        {
            if (!File.Exists(UserProfile))
                return new UpmConfigRecordCollection(new List<UpmConfigRecord>());

            using var stream = new FileStream(
                            path: UserProfile,
                            mode: FileMode.Open,
                            access: FileAccess.Read,
                            share: FileShare.ReadWrite | FileShare.Delete
                        );

            using var reader = new StreamReader(stream);
            var toml = reader.ReadToEnd();

            if (!Toml.TryToModel(toml, out TomlTable model, out var diagnostics))
            {
                Debug.LogError("TOML parse failed:");
                foreach (var diag in diagnostics)
                    Debug.LogError(diag.ToString());
            }

            var list = new List<UpmConfigRecord>();
            if (model["npmAuth"] is TomlTable npmAuthTable)
            {
                foreach (var registryEntry in npmAuthTable)
                {
                    string endPoint = registryEntry.Key;
                    if (!Uri.TryCreate(endPoint, UriKind.Absolute, out var uri))
                        continue;

                    var registry = new UpmConfigRecord()
                    {
                        EndPoint = endPoint
                    };

                    if (registryEntry.Value is TomlTable entry)
                    {
                        if (entry.TryGetValue("token", out var token))
                            registry.Token = token.ToString();
                        else
                            continue;

                        if (entry.TryGetValue("email", out var email))
                            registry.Email = email.ToString();

                        if (entry.TryGetValue("alwaysAuth", out var alwaysAuth) && alwaysAuth is bool alwaysAuthBool)
                            registry.AlwaysAuth = alwaysAuthBool;
                    }
                    list.Add(registry);
                }
            }
            return new UpmConfigRecordCollection(list);
        }

        public void Save()
        {
            var model = new TomlTable();
            var npmAuthTable = new TomlTable();

            foreach (var registry in Data)
            {
                var entry = new TomlTable
                {
                    ["token"] = registry.Token,
                    ["email"] = registry.Email,
                    ["alwaysAuth"] = registry.AlwaysAuth ?? true
                };

                npmAuthTable[registry.EndPoint] = entry;
            }

            model["npmAuth"] = npmAuthTable;

            string tomlContent = Toml.FromModel(model);
            string userProfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".upmconfig.toml");
            File.WriteAllText(userProfile, tomlContent);
        }

    }
}
