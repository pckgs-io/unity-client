using System;
using System.IO;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class ProjectPackageUIController : UIController, IBindable<ProjectPackage>
    {
        public Label DisplayNameLabel { get; private set; }
        public Label NameLabel { get; private set; }
        public Label VersionLabel { get; private set; }
        public Label DescriptionLabel { get; private set; }
        public Label ErrorLabel { get; private set; }
        public Button UploadButton { get; private set; }
        public Spinner UploadIndicator { get; private set; }

        private string _error;
        public string Error
        {
            get => _error;
            set
            {
                _error = value;
                ErrorLabel.style.display = string.IsNullOrEmpty(_error) ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }
        private bool _isUploading;
        public bool IsUploading
        {
            get => _isUploading;
            set
            {
                _isUploading = value;
                UploadButton.style.display = _isUploading ? DisplayStyle.None : DisplayStyle.Flex;
                UploadIndicator.style.display = _isUploading ? DisplayStyle.Flex : DisplayStyle.None;

            }
        }

        private ProjectPackage Package { get; set; }
        public ProjectPackageUIController(VisualElement target) : base(target)
        {
            DisplayNameLabel = target.Q<Label>("DisplayNameLabel");
            NameLabel = target.Q<Label>("NameLabel");
            VersionLabel = target.Q<Label>("VersionLabel");
            DescriptionLabel = target.Q<Label>("DescriptionLabel");
            ErrorLabel = target.Q<Label>("ErrorLabel");
            UploadButton = target.Q<Button>("UploadButton");
            UploadIndicator = target.Q<Spinner>("UploadIndicator");

            UploadButton.clicked += Upload;
            NameLabel.RegisterCallback<ClickEvent>(e =>
            {
                if (e.target == NameLabel && Package?.Path != null)
                    Application.OpenURL("file://" + Package.FullPath);
            });

            Error = null;
            IsUploading = false;
        }

        public void Bind(ProjectPackage package)
        {
            Package = package;
            DisplayNameLabel.text = package.Metadata.DisplayName ?? package.Metadata.Name;
            NameLabel.text = $"<u>{package.Metadata.Name}</u>";
            VersionLabel.text = "v" + package.Metadata.Version;
            DescriptionLabel.text = package.Metadata.Description;
            DescriptionLabel.style.visibility = string.IsNullOrEmpty(package.Metadata.Description) ? Visibility.Hidden : Visibility.Visible;
        }

        void Upload()
        {
            Error = null;
            IsUploading = false;

            var token = PckgsWindow.AccessToken;
            if (token == null)
            {
                var dialog = PckgsWindow.ShowPopup<BoolDialogUIController>(PckgsWindow.BoolDialogUIAsset);

                dialog.Header = "No Access Token";
                dialog.Description = $"No access token found on this machine to upload the package.\n\nGenerate one from the dashboard and add it on access panel to continue";
                dialog.NegativeButton.style.display = DisplayStyle.None;
                dialog.ActionSpacing.style.display = DisplayStyle.None;
                dialog.PositiveButton.text = "Close";
                return;
            }

            var uploadDialog = PckgsWindow.ShowPopup<UploadPackageDialogUIController>(PckgsWindow.UploadPackageDialogUIAsset);
            uploadDialog.OnResult += (r) =>
            {
                if (r.HasValue && r.Value)
                    UploadTo(Package, uploadDialog.VisibilityDropdown.value == "Public");
            };
            uploadDialog.Bind(Package);
        }

        async void UploadTo(ProjectPackage package, bool isPublic)
        {
            IsUploading = true;

            try
            {
                var inputFolder = Path.GetDirectoryName(package.FullPath);
                var packageJsonFile = Path.Combine(inputFolder, "package.json");

                var outputFile = await PackageManagerHelper.Pack(inputFolder, Application.temporaryCachePath);

                var tarball = File.ReadAllBytes(outputFile);
                var metadata = File.ReadAllText(packageJsonFile);

                Debug.Log("Publishing package " + package.Metadata.Name);
                try
                {
                    var api = new PckgsApi();
                    var unityPackage = await api.Packages.PublishPackage(metadata, tarball, isPublic);
                    Debug.Log("Published " + unityPackage.Name);
                }
                finally
                {
                    if (File.Exists(outputFile)) File.Delete(outputFile);
                }
            }
            catch (HttpException e)
            {
                if (e.Problem.Errors != null)
                {
                    foreach (var error in e.Problem.Errors)
                        Debug.LogError(string.Join(", ", error.Value));
                }
                else Debug.LogError(e.Problem.Detail ?? e.Problem.Title);
            }
            finally
            {
                IsUploading = false;
            }
        }

    }

}
