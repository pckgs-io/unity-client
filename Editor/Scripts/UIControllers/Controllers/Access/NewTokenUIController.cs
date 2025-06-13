using System;
using System.Linq;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class NewTokenUIController : PopupUIController
    {
        public Label ErrorLabel { get; private set; }
        public Button AddButton { get; private set; }
        public TextField TokenInputField { get; private set; }
        public Spinner ProcessingIndicator { get; private set; }

        private bool _isProcessing;
        public bool IsProcessing
        {
            get => _isProcessing;
            private set
            {
                _isProcessing = value;
                TokenInputField.SetEnabled(!_isProcessing);
                AddButton.style.visibility = _isProcessing ? Visibility.Hidden : Visibility.Visible;
                ProcessingIndicator.style.display = _isProcessing ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private string _error;
        public string Error
        {
            get => _error;
            private set
            {
                _error = value;
                ErrorLabel.text = _error;
                ErrorLabel.style.visibility = Visibility.Hidden;
                ErrorLabel.schedule.Execute(() =>
                {
                    ErrorLabel.style.visibility = string.IsNullOrEmpty(_error) ? Visibility.Hidden : Visibility.Visible;
                }).StartingIn(50);
            }
        }

        public NewTokenUIController(VisualElement target) : base(target)
        {
            ErrorLabel = target.Q<Label>("ErrorLabel");
            AddButton = target.Q<Button>("AddButton");
            ProcessingIndicator = target.Q<Spinner>("ProcessingIndicator");
            TokenInputField = target.Q<TextField>("TokenInputField");
            TokenInputField.RegisterValueChangedCallback(e =>
            {
                Error = null;
            });

            AddButton.clicked += OnAddButtonClicked;
            Error = null;
            IsProcessing = false;
        }

        private async void OnAddButtonClicked()
        {
            IsProcessing = true;
            try
            {
                var tokenInput = TokenInputField.value;
                if (string.IsNullOrEmpty(tokenInput.Trim()))
                {
                    Error = "Token is required";
                    return;
                }

                if (PckgsWindow.UpmConfigs.Data.Any(c => c.Token == tokenInput && c.EndPoint == Routes.UnityRegistry))
                {
                    Error = "Token is already added";
                    return;
                }

                Error = null;
                var token = await new PckgsApi().AccessTokens.GetAccessTokenDetails(tokenInput);

                PckgsWindow.UpmConfigs.Add(new UpmConfigRecord
                {
                    Email = token.UserEmail,
                    Token = tokenInput,
                    EndPoint = Routes.UnityRegistry
                });
                Close();
            }
            catch (HttpException e)
            {
                if (e.Problem.Status == 400 && (e.Problem.Errors?.TryGetValue("token", out var apiError) ?? false))
                    Error = string.Join(", ", apiError);
                else
                    Error = e.Problem.Detail ?? e.Problem.Title;
            }
            catch (ApiConnectionException e)
            {
                Error = e.Message;
            }
            finally
            {
                IsProcessing = false;
            }
            PckgsWindow.UpmConfigs.Save();
        }
    }

}
