using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class UploadUIController : UIController
    {
        public Button RefreshButton { get; private set; }
        public VisualElement NoPackage { get; private set; }
        public VisualElement PackageContent { get; private set; }
        public ScrollView PackageList { get; private set; }

        private readonly List<PackageUIController> packageUIControllers = new();
        public class UnityPackageListener : AssetPostprocessor
        {
            public static Action OnPackageFileUpdated;

            public static void OnPostprocessAllAssets(string[] importedAssets,
                string[] deletedAssets,
                string[] movedAssets,
                string[] movedFromAssetPaths)
            {
                if (OnPackageFileUpdated == null) return;

                var sources = new List<string[]>()
                {
                    importedAssets, deletedAssets, movedAssets, movedFromAssetPaths
                };
                foreach (var source in sources)
                {
                    if (Check(source))
                    {
                        OnPackageFileUpdated();
                        return;
                    }
                }
                bool Check(string[] source)
                {
                    foreach (string assetPath in source)
                    {
                        if (assetPath.EndsWith("package.json"))
                            return true;
                    }
                    return false;
                }
            }
        }

        public UploadUIController(VisualElement target) : base(target)
        {
            NoPackage = target.Q<VisualElement>("NoPackage");
            PackageContent = target.Q<VisualElement>("Packages");
            PackageList = target.Q<ScrollView>("PackageList");
            RefreshButton = target.Q<Button>("RefreshButton");
            RefreshButton.clicked += () =>
            {
                NoPackage.style.visibility = Visibility.Hidden;
                PackageContent.style.visibility = Visibility.Hidden;

                var scheduleItem = RefreshButton.schedule.Execute(() =>
                {
                    Refresh();
                    NoPackage.style.visibility = Visibility.Visible;
                    PackageContent.style.visibility = Visibility.Visible;
                }).Until(() => true).StartingIn(125);
            };
            Refresh();

            UnityPackageListener.OnPackageFileUpdated += Refresh;
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            UnityPackageListener.OnPackageFileUpdated -= Refresh;
            foreach (var packageUI in packageUIControllers)
                packageUI.Dispose();
        }

        void Refresh()
        {
            PackageList.Clear();
            foreach (var packageUI in packageUIControllers)
                packageUI.Dispose();
            packageUIControllers.Clear();

            var packages = ProjectPackageCollection.Load().Data;
            var hasContent = packages.Any();
            NoPackage.style.display = hasContent ? DisplayStyle.None : DisplayStyle.Flex;
            PackageContent.style.display = hasContent ? DisplayStyle.Flex : DisplayStyle.None;

            foreach (var package in packages)
            {
                var ui = PckgsWindow.PackageUIAsset.Instantiate();
                PackageList.Add(ui);
                var controller = new PackageUIController(ui);
                packageUIControllers.Add(controller);
                controller.Bind(package);
            }
        }

    }
}
