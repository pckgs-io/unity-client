using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class PackageUIController : UIController, IBindable<UnityPackage>
    {
        public Label NameLabel { get; private set; }
        public DropdownField VersionDropdown { get; private set; }
        public Button InstallButton { get; private set; }
        public Button RemoveButton { get; private set; }
        public Label InstallTypeLabel { get; private set; }

        private UnityPackage package;
        private UnityEditor.PackageManager.PackageInfo packageInfo;
        public PackageUIController(VisualElement target) : base(target)
        {
            NameLabel = target.Q<Label>("NameLabel");
            VersionDropdown = target.Q<DropdownField>("VersionDropdown");
            InstallButton = target.Q<Button>("InstallButton");
            RemoveButton = target.Q<Button>("RemoveButton");
            InstallTypeLabel = target.Q<Label>("InstallTypeLabel");

            VersionDropdown.RegisterValueChangedCallback(e => Refresh());

            InstallButton.clicked += async () =>
            {
                if (packageInfo != null && packageInfo.isDirectDependency)
                    await PackageManagerHelper.Remove(Routes.PackageName(package.Name));
                await Task.Delay(1000);
                packageInfo = await PackageManagerHelper.Add(Routes.PackageName(package.Name), VersionDropdown.text);
                Refresh();
            };
            RemoveButton.clicked += async () =>
            {
                await PackageManagerHelper.Remove(Routes.PackageName(package.Name));
                await Task.Delay(100);
                packageInfo = UnityEditor.PackageManager.PackageInfo.FindForPackageName(Routes.PackageName(package.Name));
                Refresh();
            };
        }

        public void Bind(UnityPackage obj)
        {
            package = obj;
            packageInfo = UnityEditor.PackageManager.PackageInfo.FindForPackageName(Routes.PackageName(obj.Name));

            NameLabel.text = $"<b>{obj.Name}</b> -  <i>{(obj.IsPublic ? "public" : "private")}</i>";
            var versions = obj.Releases.Select(r => r.Version.NormalizedString).ToList();
            VersionDropdown.choices = versions;
            VersionDropdown.index = versions.Count - 1;

        }

        void Refresh()
        {
            var version = VersionDropdown.value;
            var isInstalled = packageInfo != null && packageInfo.version == version;
            var isDirectDependency = isInstalled && packageInfo.isDirectDependency;

            InstallTypeLabel.text = $"<i>{(!isInstalled ? "Not installed" : isDirectDependency ? "Installed" : "Installed as dependency")}</i>";
            InstallButton.style.display = !isInstalled || !isDirectDependency ? DisplayStyle.Flex : DisplayStyle.None;
            RemoveButton.style.display = !isInstalled || !isDirectDependency ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }

}
