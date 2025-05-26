using System;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class PackageAccessUIController : UIController, IBindable<AccessTokenPackageAccess>
    {
        public Label NameLabel { get; private set; }
        public Toggle ReadToggle { get; private set; }
        public Toggle WriteToggle { get; private set; }

        public PackageAccessUIController(VisualElement target) : base(target)
        {
            NameLabel = target.Q<Label>("NameLabel");
            ReadToggle = target.Q<Toggle>("ReadToggle");
            WriteToggle = target.Q<Toggle>("WriteToggle");

            ReadToggle.SetEnabled(false);
            WriteToggle.SetEnabled(false);
        }

        public void Bind(AccessTokenPackageAccess obj)
        {
            NameLabel.text = obj.PackageName;
            ReadToggle.value = obj.Read;
            WriteToggle.value = obj.Write;
        }
    }

}
