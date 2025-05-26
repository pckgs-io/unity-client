using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class OrganizationAccessUIController : UIController, IBindable<AccessTokenOrganizationAccess>, IBindable<AccessTokenOrganizationDetail>
    {
        public Label NameLabel { get; private set; }
        public VisualElement PackageList { get; private set; }
        public Toggle AllowNewPackageToggle { get; private set; }

        private string url;
        public OrganizationAccessUIController(VisualElement target) : base(target)
        {
            NameLabel = target.Q<Label>("NameLabel");
            AllowNewPackageToggle = target.Q<Toggle>("AllowNewPackageToggle");
            PackageList = target.Q<VisualElement>("PackageList");
            NameLabel.RegisterCallback<ClickEvent>(e =>
            {
                if (e.target == NameLabel && !string.IsNullOrEmpty(url))
                    Application.OpenURL(url);
            });
            AllowNewPackageToggle.SetEnabled(false);
        }

        public void Bind(AccessTokenOrganizationAccess obj)
        {
            AllowNewPackageToggle.value = obj.AllowCreatePackage;

            if (obj.PackageAccesses != null)
            {
                foreach (var access in obj.PackageAccesses)
                {
                    var ui = PckgsWindow.PackageAccessUIAsset.Instantiate();
                    var controller = new PackageAccessUIController(ui);
                    PackageList.Add(ui);
                    controller.Bind(access);
                }
            }
        }

        public void Bind(AccessTokenOrganizationDetail obj)
        {
            NameLabel.text = $"<u>{obj.Name}</u>";
            url = Routes.OrganizationWebsite(obj.Slug);
        }
    }

}
