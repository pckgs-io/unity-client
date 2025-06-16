using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class UploadPackageDialogUIController : BoolDialogUIController, IBindable<ProjectPackage>
    {
        public VisualElement ActionContainer { get; private set; }
        public Spinner BottomSpinner { get; private set; }
        public DropdownField VisibilityDropdown { get; private set; }
        public Label VisibilityWarning { get; private set; }


        private bool _blockActions;
        public bool BlockActions
        {
            get => _blockActions;
            set
            {
                _blockActions = value;
                ActionContainer.style.display = _blockActions ? DisplayStyle.None : DisplayStyle.Flex;
                BottomSpinner.style.display = _blockActions ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public UploadPackageDialogUIController(VisualElement target) : base(target)
        {
            ActionContainer = target.Q<VisualElement>("Actions");
            BottomSpinner = target.Q<Spinner>("BottomSpinner");
            VisibilityDropdown = target.Q<DropdownField>("VisibilityDropdown");
            VisibilityWarning = target.Q<Label>("VisibilityWarning");

            BlockActions = false;
        }

        public async void Bind(ProjectPackage package)
        {
            string orgSlug;
            try
            {
                orgSlug = package.Metadata.Name.Split('.', StringSplitOptions.RemoveEmptyEntries)[1];
            }
            catch
            {
                Debug.LogError($"Package name {package.Metadata.Name} is invalid");
                return;
            }

            Header = $"Upload {package.Metadata.Name}";
            Description = $"Do you want to upload <b>{package.Metadata.Name}@{package.Metadata.Version}</b> to organization <b>{orgSlug}</b>?";
            NegativeButton.text = "Cancel";
            PositiveButton.text = "Upload";

            BlockActions = true;

            PositiveButton.SetEnabled(false);
            VisibilityDropdown.RegisterValueChangedCallback(e =>
            {
                var hasValue = !string.IsNullOrEmpty(e.newValue);
                PositiveButton.SetEnabled(hasValue);
                VisibilityWarning.style.display = hasValue ? DisplayStyle.None : DisplayStyle.Flex;
            });
            VisibilityDropdown.choices = new System.Collections.Generic.List<string>
            {
                "Public",
                "Private"
            };

            Content.style.display = DisplayStyle.None;
            try
            {
                var api = new PckgsApi();
                var p = await api.Packages.Get(package.Metadata.Name);
                PositiveButton.SetEnabled(p != null);
                Content.style.display = p != null ? DisplayStyle.None : DisplayStyle.Flex;
            }
            catch
            {
                Content.style.display = DisplayStyle.Flex;
                PositiveButton.SetEnabled(false);
            }
            finally
            {
                BlockActions = false;
            }
        }
    }
}
