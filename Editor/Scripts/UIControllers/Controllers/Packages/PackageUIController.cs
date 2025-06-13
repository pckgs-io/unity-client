using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
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
        public VisualElement Actions { get; private set; }
        public VisualElement InstallSection { get; private set; }
        public Label LockedLabel { get; private set; }
        public Spinner Spinner { get; private set; }

        bool _isProcessing;
        public bool IsProcessing
        {
            get => _isProcessing;
            private set
            {
                _isProcessing = value;
                Actions.style.display = _isProcessing ? DisplayStyle.None : DisplayStyle.Flex;
                Spinner.style.display = _isProcessing ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private UnityPackage package;
        private UnityEditor.PackageManager.PackageInfo packageInfo;
        public PackageUIController(VisualElement target) : base(target)
        {
            NameLabel = target.Q<Label>("NameLabel");
            VersionDropdown = target.Q<DropdownField>("VersionDropdown");
            InstallButton = target.Q<Button>("InstallButton");
            InstallSection = target.Q<VisualElement>("InstallSection");
            LockedLabel = target.Q<Label>("LockedLabel");
            RemoveButton = target.Q<Button>("RemoveButton");
            InstallTypeLabel = target.Q<Label>("InstallTypeLabel");
            Actions = target.Q<VisualElement>("Actions");
            Spinner = target.Q<Spinner>("Spinner");

            IsProcessing = false;

            VersionDropdown.RegisterValueChangedCallback(e => Refresh());

            InstallButton.clicked += async () =>
            {
                try
                {
                    IsProcessing = true;
                    if (packageInfo != null && packageInfo.isDirectDependency)
                        await PackageManagerHelper.Remove(Routes.PackageName(package.Name));
                    await Task.Delay(500);
                    packageInfo = await PackageManagerHelper.Add(Routes.PackageName(package.Name), VersionDropdown.text);
                }
                finally
                {
                    IsProcessing = false;
                }
                Refresh();
            };
            RemoveButton.clicked += async () =>
            {
                try
                {
                    IsProcessing = true;
                    await PackageManagerHelper.Remove(Routes.PackageName(package.Name));
                    await Task.Delay(500);
                    packageInfo = UnityEditor.PackageManager.PackageInfo.FindForPackageName(Routes.PackageName(package.Name));
                }
                finally
                {
                    IsProcessing = false;
                }
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
            InstallButton.style.display = obj.IsLocked ? DisplayStyle.None : DisplayStyle.Flex;
            LockedLabel.style.display = obj.IsLocked ? DisplayStyle.Flex : DisplayStyle.None;
        }

        void Refresh()
        {
            var version = VersionDropdown.value;
            var isInstalled = packageInfo != null && packageInfo.version == version;
            var isDirectDependency = isInstalled && packageInfo.isDirectDependency;

            InstallTypeLabel.text = $"<i>{(!isInstalled ? "Not installed" : isDirectDependency ? "Installed" : "Installed as dependency")}</i>";
            InstallSection.style.display = !isInstalled || !isDirectDependency ? DisplayStyle.Flex : DisplayStyle.None;
            RemoveButton.style.display = !isInstalled || !isDirectDependency ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }

}
