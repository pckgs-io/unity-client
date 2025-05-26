using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class AccessTokenUIController : UIController, IBindable<string>, IBindable<AccessTokenDetails>
    {
        public Spinner LoadingIndicator { get; private set; }
        public Label ErrorLabel { get; private set; }
        public VisualElement Content { get; private set; }
        public VisualElement ResultView { get; private set; }
        public VisualElement ErrorView { get; private set; }
        public Button ErrorRemoveButton { get; private set; }

        public Label NameLabel { get; private set; }
        public Label CreatedAtLabel { get; private set; }
        public Label ExpirationLabel { get; private set; }
        public Label NoAccessLabel { get; private set; }
        public Button RemoveButton { get; private set; }
        public ScrollView OrganizationAccessList { get; private set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                _isLoading = value;
                LoadingIndicator.style.display = _isLoading ? DisplayStyle.Flex : DisplayStyle.None;
                ResultView.style.display = _isLoading ? DisplayStyle.None : DisplayStyle.Flex;
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
                ErrorLabel.text = _error;
                var isNull = string.IsNullOrEmpty(_error);
                ErrorView.style.display = isNull ? DisplayStyle.None : DisplayStyle.Flex;
                Content.style.display = isNull ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public AccessTokenDetails AccessTokenDetails { get; private set; }

        public AccessTokenUIController(VisualElement target) : base(target)
        {
            LoadingIndicator = target.Q<Spinner>("LoadingIndicator");
            ErrorLabel = target.Q<Label>("Error");
            ErrorView = target.Q<VisualElement>("ErrorView");
            ErrorRemoveButton = target.Q<Button>("ErrorRemoveButton");
            Content = target.Q<VisualElement>("Content");
            NameLabel = target.Q<Label>("NameLabel");
            ResultView = target.Q<VisualElement>("ResultView");
            RemoveButton = target.Q<Button>("RemoveButton");
            CreatedAtLabel = target.Q<Label>("CreatedAtLabel");
            ExpirationLabel = target.Q<Label>("ExpirationLabel");
            OrganizationAccessList = target.Q<ScrollView>("OrganizationAccessList");
            NoAccessLabel = target.Q<Label>("NoAccessLabel");

            NameLabel.RegisterCallback<ClickEvent>(e =>
            {
                if (e.target == NameLabel && !string.IsNullOrEmpty(AccessTokenDetails.AccessToken.Slug))
                    Application.OpenURL(Routes.AccessTokenWebsite(AccessTokenDetails.AccessToken.Slug));
            });

            RemoveButton.clicked += RemoveAccessToken;
            ErrorRemoveButton.clicked += RemoveAccessToken;

            Error = null;
            IsLoading = false;
        }

        private void RemoveAccessToken()
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
        }

        public async void Bind(string obj)
        {
            Token = obj;
            Error = null;
            IsLoading = true;

            try
            {
                var details = await new PckgsApi().AccessTokens.GetAccessTokenDetails(obj);
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
            ExpirationLabel.style.display = details.AccessToken.ExpirationDate != null ? DisplayStyle.Flex : DisplayStyle.None;
            CreatedAtLabel.text = "Created at " + details.AccessToken.CreationDate.ToString("dd MMMM yyyy");
            if (details.AccessToken?.ExpirationDate != null)
                ExpirationLabel.text = GetExpirationStatus(details.AccessToken.ExpirationDate.Value);

            OrganizationAccessList.Clear();
            var hasAccess = details.AccessToken.Accesses != null && details.AccessToken.Accesses.Any();
            NoAccessLabel.style.display = hasAccess ? DisplayStyle.None : DisplayStyle.Flex;
            OrganizationAccessList.style.display = hasAccess ? DisplayStyle.Flex : DisplayStyle.None;

            if (hasAccess)
            {
                foreach (var access in details.AccessToken.Accesses)
                {
                    var controller = CloneTree<OrganizationAccessUIController>(
                        PckgsWindow.OrganizationAccessUIAsset,
                        OrganizationAccessList);

                    controller.Bind(access);
                    if (details.Organizations.TryGetValue(access.OrganizationId, out var orgDetails))
                        controller.Bind(orgDetails);
                }
            }

            Token = details.AccessToken.Token;
            AccessTokenDetails = details;
            NameLabel.text = $"<u>{details.AccessToken.Name}</u>";
        }

        public string GetExpirationStatus(DateTime targetDateUtc)
        {
            var now = DateTime.UtcNow;
            var timeLeft = targetDateUtc - now;

            if (timeLeft > TimeSpan.Zero)
            {
                if (timeLeft.TotalDays >= 30)
                {
                    int monthsLeft = (int)(timeLeft.TotalDays / 30);
                    return $"Expires in {monthsLeft} month{(monthsLeft > 1 ? "s" : "")}";
                }
                else
                {
                    int daysLeft = (int)Math.Ceiling(timeLeft.TotalDays);
                    return $"Expires in {daysLeft} day{(daysLeft > 1 ? "s" : "")}";
                }
            }
            else
            {
                return "Expired";
            }
        }

    }

}
