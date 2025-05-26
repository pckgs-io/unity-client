using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class WindowUIController : TabUIController
    {
        public Tab PackagesTab { get; private set; }
        public Tab AccessTab { get; private set; }
        public Tab UploadTab { get; private set; }

        public WindowUIController(VisualElement target) : base(target)
        {
            PackagesTab = target.Q<Tab>("PackagesTab");
            AccessTab = target.Q<Tab>("AccessTab");
            UploadTab = target.Q<Tab>("UploadTab");

            PackagesTab.selected += (t) =>
            {
                ActivateTab<PackagesUIController>(PckgsWindow.PackagesUIAsset, PackagesTab);
            };
            AccessTab.selected += (t) =>
            {
                ActivateTab<AccessUIController>(PckgsWindow.AccessUIAsset, AccessTab);
            };
            UploadTab.selected += (t) =>
            {
                ActivateTab<UploadUIController>(PckgsWindow.UploadUIAsset, UploadTab);
            };
            TabView.activeTab = PackagesTab;
            ActivateTab<PackagesUIController>(PckgsWindow.PackagesUIAsset, PackagesTab);
        }

    }
}
