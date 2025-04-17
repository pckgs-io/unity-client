using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class AccessTokenUIController : UIController, IBindable<string>, IBindable<AccessTokenDetails>
    {
        public Label NameLabel { get; private set; }
        public Label ExpirationLabel { get; private set; }
        public Spinner LoadingIndicator { get; private set; }
        public VisualElement TokenContent { get; private set; }
        public VisualElement Content { get; private set; }
        public VisualElement OrgLogo { get; private set; }
        public Label OrgNameLabel { get; private set; }
        public Label DetailsError { get; private set; }
        public Button RemoveButton { get; private set; }
        public Button RegistryButton { get; private set; }

        public FileDataUIController OrgLogoController { get; private set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                _isLoading = value;
                LoadingIndicator.style.display = _isLoading ? DisplayStyle.Flex : DisplayStyle.None;
                Content.style.display = _isLoading ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        private AccessTokenDetails _details;
        public AccessTokenDetails Details
        {
            get => _details;
            set
            {
                _details = value;
            }
        }
        private string _token;
        public string Token
        {
            get => _token;
            set => _token = value;
        }

        private string _error;
        public string Error
        {
            get => _error;
            set
            {
                _error = value;
                DetailsError.text = _error;
                var isNull = string.IsNullOrEmpty(_error);
                DetailsError.style.display = isNull ? DisplayStyle.None : DisplayStyle.Flex;
                TokenContent.style.display = isNull ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public AccessTokenDetails AccessTokenDetails { get; private set; }

        public AccessTokenUIController(VisualElement target) : base(target)
        {
            NameLabel = target.Q<Label>("NameLabel");
            LoadingIndicator = target.Q<Spinner>("LoadingIndicator");
            Content = target.Q<VisualElement>("Content");
            TokenContent = target.Q<VisualElement>("TokenContent");
            OrgLogo = target.Q<VisualElement>("OrgLogo");
            OrgNameLabel = target.Q<Label>("OrgName");
            DetailsError = target.Q<Label>("Error");
            RemoveButton = target.Q<Button>("RemoveButton");
            RegistryButton = target.Q<Button>("RegistryButton");
            ExpirationLabel = target.Q<Label>("ExpirationLabel");

            var orgContent = target.Q<VisualElement>("OrgContent");
            orgContent.RegisterCallback<ClickEvent>(e =>
            {
                Application.OpenURL(Navigation.OrganizationWebsite(AccessTokenDetails.OrganizationSlug));
            });
            OrgLogo.RegisterCallback<GeometryChangedEvent>(e =>
            {
                OrgLogo.style.width = OrgLogo.resolvedStyle.height;
            });

            RemoveButton.clicked += () =>
            {
                var name = AccessTokenDetails?.AccessToken?.Name;

                var dialog = PckgsWindow.ShowPopup<BoolDialogUIController>(PckgsWindow.BoolDialogUIAsset);
                dialog.Header = name != null ? $"Remove {name}" : "Remove Access Token";
                dialog.Description = "The token will be removed from this device, but it remains valid on the server and usable from other systems. Continue?";
                dialog.NegativeButton.text = "Cancel";
                dialog.PositiveButton.text = "Remove";

                dialog.OnResult += (result) =>
                {
                    if (result.HasValue && result.Value)
                    {
                        var data = PckgsWindow.UpmConfigs.Data.FirstOrDefault(d => d.Token == Token);
                        if (data != null)
                        {
                            PckgsWindow.UpmConfigs.Remove(data);
                            PckgsWindow.UpmConfigs.Save();
                        }
                    }
                };
            };
            RegistryButton.clicked += () =>
            {
                if (string.IsNullOrEmpty(AccessTokenDetails.OrganizationSlug)) throw new ArgumentNullException(nameof(AccessTokenDetails.OrganizationSlug));
                var url = Navigation.OrganizationRegistry(AccessTokenDetails.OrganizationSlug);
                var registry = PckgsWindow.ScopedRegistries.Data.FirstOrDefault(s => s.Url == url);
                if (registry != null)
                {
                    PckgsWindow.ScopedRegistries.Remove(registry);
                    PckgsWindow.ScopedRegistries.Save();
                    UnityEditor.PackageManager.Client.Resolve();
                    Debug.Log($"Scoped registry {url} removed from project");
                }
                else
                {
                    registry = new UnityScopedRegistry
                    {
                        Name = AccessTokenDetails.OrganizationName,
                        Url = url,
                        Scopes = new List<string> { "*" }
                    };
                    PckgsWindow.ScopedRegistries.Add(registry);
                    PckgsWindow.ScopedRegistries.Save();
                    UnityEditor.PackageManager.Client.Resolve();
                    Debug.Log($"Scoped registry {url} added to project");
                }
                RefreshRegistryButtonUI();
            };

            OrgLogoController = new FileDataUIController(OrgLogo);
            OrgLogoController.Bind(null);
            Error = null;
            IsLoading = false;
            RegistryButton.style.display = DisplayStyle.None;
        }

        void RefreshRegistryButtonUI()
        {
            var registryUrl = Navigation.OrganizationRegistry(AccessTokenDetails.OrganizationSlug);
            var registry = PckgsWindow.ScopedRegistries.Data.FirstOrDefault(s => s.Url == registryUrl);
            if (registry != null)
            {
                RegistryButton.text = "<u>Remove Registry</u>";
                if (RegistryButton.ClassListContains("link"))
                    RegistryButton.RemoveFromClassList("link");
                RegistryButton.AddToClassList("link-destructive");
            }
            else
            {
                RegistryButton.text = "<u>Install Registry</u>";

                if (RegistryButton.ClassListContains("link-destructive"))
                    RegistryButton.RemoveFromClassList("link-destructive");
                RegistryButton.AddToClassList("link");
            }
        }

        public async void Bind(string obj)
        {
            Token = obj;
            Error = null;
            IsLoading = true;

            try
            {
                var details = await PckgsApi.GetAccessTokenDetails(obj);
                Bind(details);
            }
            catch (Exception e)
            {
                Error = e.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void Bind(AccessTokenDetails details)
        {
            RegistryButton.style.display = details != null ? DisplayStyle.Flex : DisplayStyle.None;

            ExpirationLabel.style.display = details.AccessToken.ExpirationDate != null ? DisplayStyle.Flex : DisplayStyle.None;
            if (details.AccessToken?.ExpirationDate != null)
                ExpirationLabel.text = GetExpirationStatus(details.AccessToken.ExpirationDate.Value);
            Token = details.AccessToken.Token;
            AccessTokenDetails = details;
            NameLabel.text = details.AccessToken.Name;
            OrgNameLabel.text = $"<u>{details.OrganizationName}</u>";
            OrgLogoController.Bind(details.OrganizationLogo);
            RefreshRegistryButtonUI();
        }

        public string GetExpirationStatus(DateTime targetDateUtc)
        {
            // Calculate the difference between the target date and the current UTC date
            TimeSpan timeLeft = targetDateUtc - DateTime.UtcNow;

            // If the expiration date is in the future
            if (timeLeft > TimeSpan.Zero)
            {
                // Calculate months and days
                int monthsLeft = targetDateUtc.Month - DateTime.UtcNow.Month + 12 * (targetDateUtc.Year - DateTime.UtcNow.Year);
                if (monthsLeft > 0)
                {
                    return $"Expires in {monthsLeft} month{(monthsLeft > 1 ? "s" : "")}";
                }
                else
                {
                    // Calculate remaining days if less than a month
                    int daysLeft = (int)Math.Ceiling(timeLeft.TotalDays);
                    return $"Expires in {daysLeft} day{(daysLeft > 1 ? "s" : "")}";
                }
            }
            else
            {
                // If expired, return the expired message
                return "Expired";
            }
        }
    }

}
