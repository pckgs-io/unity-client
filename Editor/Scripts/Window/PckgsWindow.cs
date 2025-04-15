using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class PckgsWindow : EditorWindow
    {
        [SerializeField] VisualTreeAsset accessUIAsset;
        [SerializeField] VisualTreeAsset accessTokenUIAsset;
        [SerializeField] VisualTreeAsset newTokenUIAsset;
        [SerializeField] VisualTreeAsset boolDialogUIAsset;

        public static VisualTreeAsset AccessUIAsset => Instance.accessUIAsset;
        public static VisualTreeAsset AccessTokenUIAsset => Instance.accessTokenUIAsset;
        public static VisualTreeAsset NewTokenUIAsset => Instance.newTokenUIAsset;
        public static VisualTreeAsset BoolDialogUIAsset => Instance.boolDialogUIAsset;

        public static UpmConfigRecordCollection UpmConfigs { get; private set; }
        public static UnityScopedRegistryCollection ScopedRegistries { get; private set; }


        private static PckgsWindow _instance;
        public static PckgsWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GetWindow<PckgsWindow>();
                    _instance.titleContent = new GUIContent("pckgs.io");
                }
                return _instance;
            }
        }

        [MenuItem("Window/pckgs.io")]
        public static void OpenWindow()
        {
            var instance = Instance;
        }

        public void CreateGUI()
        {
            ScopedRegistries = UnityScopedRegistryCollection.Load();
            UpmConfigs = UpmConfigRecordCollection.Load();
            accessUIAsset.CloneTree<AccessUIController>(rootVisualElement);
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
