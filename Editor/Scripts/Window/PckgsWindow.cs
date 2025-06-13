using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class PckgsWindow : EditorWindow
    {
        [SerializeField] Texture2D iconAsset;

        [SerializeField] VisualTreeAsset windowUIAsset;

        [Header("Access")]
        [SerializeField] VisualTreeAsset accessUIAsset;
        [SerializeField] VisualTreeAsset accessTokenUIAsset;
        [SerializeField] VisualTreeAsset organizationAccessUIAsset;
        [SerializeField] VisualTreeAsset packageAccessUIAsset;
        [SerializeField] VisualTreeAsset newTokenUIAsset;

        [Header("Upload")]
        [SerializeField] VisualTreeAsset uploadUIAsset;
        [SerializeField] VisualTreeAsset projectPackageUIAsset;

        [Header("Packages")]
        [SerializeField] VisualTreeAsset packagesUIAsset;
        [SerializeField] VisualTreeAsset searchPackagesUIAsset;
        [SerializeField] VisualTreeAsset packageUIAsset;


        [Header("")]
        [SerializeField] VisualTreeAsset boolDialogUIAsset;
        [SerializeField] VisualTreeAsset pickerUIAsset;
        [SerializeField] VisualTreeAsset pickerOptionUIAsset;
        [SerializeField] VisualTreeAsset paginatedDataUIAsset;


        public static VisualTreeAsset AccessUIAsset => Instance.accessUIAsset;
        public static VisualTreeAsset AccessTokenUIAsset => Instance.accessTokenUIAsset;
        public static VisualTreeAsset OrganizationAccessUIAsset => Instance.organizationAccessUIAsset;
        public static VisualTreeAsset PackageAccessUIAsset => Instance.packageAccessUIAsset;
        public static VisualTreeAsset UploadUIAsset => Instance.uploadUIAsset;
        public static VisualTreeAsset ProjectPackageUIAsset => Instance.projectPackageUIAsset;
        public static VisualTreeAsset NewTokenUIAsset => Instance.newTokenUIAsset;
        public static VisualTreeAsset BoolDialogUIAsset => Instance.boolDialogUIAsset;
        public static VisualTreeAsset PickerUIAsset => Instance.pickerUIAsset;
        public static VisualTreeAsset PickerOptionUIAsset => Instance.pickerOptionUIAsset;
        public static VisualTreeAsset PackagesUIAsset => Instance.packagesUIAsset;
        public static VisualTreeAsset PackageUIAsset => Instance.packageUIAsset;
        public static VisualTreeAsset SearchPackagesUIAsset => Instance.searchPackagesUIAsset;
        public static VisualTreeAsset PaginatedDataUIAsset => Instance.paginatedDataUIAsset;

        public static UpmConfigRecordCollection UpmConfigs => UpmConfigRecordCollection.Instance;
        public static UnityScopedRegistryCollection ScopedRegistries { get; private set; }

        private static PckgsWindow _instance;
        public static PckgsWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GetWindow<PckgsWindow>();
                    _instance.titleContent = new GUIContent("pckgs.io", _instance.iconAsset);
                    _instance.minSize = new Vector2(400, 400);
                }
                return _instance;
            }
        }

        public static string AccessToken => UpmConfigs.Data.FirstOrDefault(d => d.EndPoint == Routes.UnityRegistry)?.Token;
        private FileSystemWatcher _packageManifestWatcher;
        private UIController windowUIController;
        [MenuItem("Window/pckgs.io")]
        public static void OpenWindow()
        {
            var instance = Instance;
        }

        public void CreateGUI()
        {
            if (windowUIAsset == null)
            {
                Close();
                EditorApplication.delayCall += OpenWindow;
            }
            ScopedRegistries = UnityScopedRegistryCollection.Load();
            windowUIController = windowUIAsset.CloneTree<WindowUIController>(rootVisualElement);
        }
        void OnEnable()
        {
            _packageManifestWatcher = new FileSystemWatcher
            {
                Path = ProjectPackageManifest.PackagesFolderPath,
                Filter = Path.GetFileName(ProjectPackageManifest.PackageManifestFileName),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };
            _packageManifestWatcher.Created += (sender, e) =>
            {
                EditorApplication.delayCall += ScopedRegistries.Reload;
            };
            _packageManifestWatcher.Deleted += (sender, e) =>
            {
                EditorApplication.delayCall += ScopedRegistries.Reload;
            };
            _packageManifestWatcher.Changed += (sender, e) =>
            {
                EditorApplication.delayCall += ScopedRegistries.Reload;
            };

            _packageManifestWatcher.EnableRaisingEvents = true;
        }
        private void OnDestroy()
        {
            _packageManifestWatcher?.Dispose();
            windowUIController?.Dispose();
        }

        public static T ShowPopup<T>(VisualTreeAsset popupAsset) where T : PopupUIController
        {
            var element = new VisualElement();
            element.name = "Popup";
            element.style.flexGrow = 1f;
            element.style.backgroundColor = new Color(0f, 0f, 0f, 0.5f);
            element.style.position = Position.Absolute;
            element.StretchToParentSize();
            element.style.alignItems = Align.Stretch;
            element.style.justifyContent = Justify.Center;

            Instance.rootVisualElement.Add(element);

            var controller = popupAsset.CloneTree<T>(element);
            controller.OnClosed += () =>
            {
                Instance.rootVisualElement.Remove(element);
            };
            element.RegisterCallback<ClickEvent>(e =>
            {
                if (e.target == element)
                    controller.Close();
            });
            return controller;
        }
    }
}
