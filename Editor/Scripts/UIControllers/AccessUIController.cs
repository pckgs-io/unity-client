using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class AccessUIController : UIController
    {
        public VisualElement NoTokenUI { get; private set; }
        public VisualElement TokensUI { get; private set; }
        public VisualElement TokenList { get; private set; }

        public VisualElement Content { get; private set; }
        public Spinner LoadingIndicator { get; private set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                _isLoading = value;
                Content.style.display = _isLoading ? DisplayStyle.None : DisplayStyle.Flex;
                LoadingIndicator.style.display = _isLoading ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private readonly Dictionary<string, VisualElement> accessTokenUIElements = new();

        public AccessUIController(VisualElement target) : base(target)
        {
            Content = target.Q<VisualElement>("Content");
            LoadingIndicator = target.Q<Spinner>("LoadingIndicator");

            NoTokenUI = target.Q<VisualElement>("NoToken");
            TokensUI = target.Q<VisualElement>("Tokens");
            TokenList = target.Q<ScrollView>("TokenList");
            var accessDashboardLabel = target.Q<Label>("AccessDashboardLink");
            accessDashboardLabel.RegisterCallback<ClickEvent>(e =>
            {
                if (e.target == accessDashboardLabel)
                    Application.OpenURL(Navigation.AccessDashboardLink);
            });

            var button = target.Q<Button>("AddButton");
            button.clicked += OnAddButtonClicked;

            PckgsWindow.UpmConfigs.OnDataAdded += (config) =>
            {
                var ui = PckgsWindow.AccessTokenUIAsset.Instantiate();
                TokenList.Add(ui);
                accessTokenUIElements.Add(config.Token, ui);
                var controller = new AccessTokenUIController(ui);
                controller.Bind(config.Token);
                RefreshUI();
            };
            PckgsWindow.UpmConfigs.OnDataRemoved += (config) =>
            {
                if (accessTokenUIElements.TryGetValue(config.Token, out var ui))
                {
                    accessTokenUIElements.Remove(config.Token);
                    if (TokenList.Contains(ui))
                    {
                        TokenList.Remove(ui);
                        RefreshUI();
                    }
                }
            };

            RefreshData();
        }

        void OnAddButtonClicked()
        {
            PckgsWindow.ShowPopup<NewTokenUIController>(PckgsWindow.NewTokenUIAsset);
        }

        void RefreshUI()
        {
            var tokens = PckgsWindow.UpmConfigs.Data.Where(r => r.EndPoint.StartsWith(Navigation.Backend)).Select(r => r.Token);
            var hasContent = tokens.Any();
            NoTokenUI.style.display = hasContent ? DisplayStyle.None : DisplayStyle.Flex;
            TokensUI.style.display = hasContent ? DisplayStyle.Flex : DisplayStyle.None;
        }

        void RefreshData()
        {
            IsLoading = true;

            try
            {
                TokenList.Clear();
                accessTokenUIElements.Clear();
                var tokens = PckgsWindow.UpmConfigs.Data.Where(r => r.EndPoint.StartsWith(Navigation.Backend)).Select(r => r.Token);
                RefreshUI();
                foreach (var token in tokens)
                {
                    var ui = PckgsWindow.AccessTokenUIAsset.Instantiate();
                    TokenList.Add(ui);
                    accessTokenUIElements.Add(token, ui);
                    var controller = new AccessTokenUIController(ui);
                    controller.Bind(token);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

}
