
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Pckgs
{
    public class PackageManagerHelper
    {
        public static Task<UnityEditor.PackageManager.PackageInfo> Add(string packageName, string version)
        {
            var registry = PckgsWindow.ScopedRegistries.Data.FirstOrDefault(e => e.Url == Routes.UnityRegistry);
            if (registry == null)
            {
                PckgsWindow.ScopedRegistries.Add(new UnityScopedRegistry
                {
                    Name = "pckgs.io",
                    Url = Routes.UnityRegistry,
                    Scopes = new System.Collections.Generic.List<string>() { Routes.PackagePrefix }
                });
                PckgsWindow.ScopedRegistries.Save();
            }

            var completion = new TaskCompletionSource<UnityEditor.PackageManager.PackageInfo>();
            var req = UnityEditor.PackageManager.Client.Add($"{packageName}@{version}");
            var callback = default(EditorApplication.CallbackFunction);
            callback = () =>
            {
                switch (req.Status)
                {
                    case UnityEditor.PackageManager.StatusCode.InProgress:
                        return;
                    case UnityEditor.PackageManager.StatusCode.Success:
                        EditorApplication.update -= callback;
                        completion.SetResult(req.Result);
                        break;
                    case UnityEditor.PackageManager.StatusCode.Failure:
                        EditorApplication.update -= callback;
                        completion.SetException(new Exception(req.Error.message));
                        break;
                }
            };
            EditorApplication.update += callback;
            return completion.Task;
        }

        public static async Task Remove(string packageName)
        {
            var completion = new TaskCompletionSource<bool>();
            var req = UnityEditor.PackageManager.Client.Remove(packageName);
            var callback = default(EditorApplication.CallbackFunction);
            callback = () =>
            {
                switch (req.Status)
                {
                    case UnityEditor.PackageManager.StatusCode.InProgress:
                        return;
                    case UnityEditor.PackageManager.StatusCode.Success:
                        EditorApplication.update -= callback;
                        completion.SetResult(true);
                        break;
                    case UnityEditor.PackageManager.StatusCode.Failure:
                        EditorApplication.update -= callback;
                        completion.SetException(new Exception(req.Error.message));
                        break;
                }
            };
            EditorApplication.update += callback;
            await completion.Task;
        }

        public static void Resolve()
        {
            UnityEditor.PackageManager.Client.Resolve();
        }
    }
}
