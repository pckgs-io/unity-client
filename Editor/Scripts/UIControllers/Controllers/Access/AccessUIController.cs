using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class AccessUIController : UIController
    {
        public VisualElement NoTokenUI { get; private set; }
        public VisualElement AccessTokenUI { get; private set; }

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

        public AccessTokenUIController AccessTokenUIController { get; private set; }

        public AccessUIController(VisualElement target) : base(target)
        {
            Content = target.Q<VisualElement>("Content");
            LoadingIndicator = target.Q<Spinner>("LoadingIndicator");

            NoTokenUI = target.Q<VisualElement>("NoToken");
            AccessTokenUI = target.Q<VisualElement>("Token");
            var accessDashboardLabel = target.Q<Label>("AccessDashboardLink");
            accessDashboardLabel.RegisterCallback<ClickEvent>(e =>
            {
                if (e.target == accessDashboardLabel)
                    Application.OpenURL(Routes.AccessDashboardLink);
            });

            var button = target.Q<Button>("AddButton");
            button.clicked += OnAddButtonClicked;

            PckgsWindow.UpmConfigs.Reload();
            PckgsWindow.UpmConfigs.OnDataAdded += OnDataAdded;
            PckgsWindow.UpmConfigs.OnDataRemoved += OnDataRemoved;

            RefreshData();
        }

        private void OnDataRemoved(UpmConfigRecord record)
        {
            RefreshData();
        }

        private void OnDataAdded(UpmConfigRecord record)
        {
            RefreshData();
        }

        void OnAddButtonClicked()
        {
            PckgsWindow.ShowPopup<NewTokenUIController>(PckgsWindow.NewTokenUIAsset);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            PckgsWindow.UpmConfigs.OnDataAdded -= OnDataAdded;
            PckgsWindow.UpmConfigs.OnDataRemoved -= OnDataRemoved;
            AccessTokenUIController?.Dispose();
        }

        public void RefreshData()
        {
            IsLoading = true;

            try
            {
                AccessTokenUI.Clear();
                AccessTokenUIController?.Dispose();

                var token = PckgsWindow.UpmConfigs.Data.FirstOrDefault(r => r.EndPoint == Routes.UnityRegistry)?.Token;

                var hasToken = !string.IsNullOrEmpty(token);
                NoTokenUI.style.display = hasToken ? DisplayStyle.None : DisplayStyle.Flex;
                AccessTokenUI.style.display = hasToken ? DisplayStyle.Flex : DisplayStyle.None;

                AccessTokenUIController = PckgsWindow.AccessTokenUIAsset.CloneTree<AccessTokenUIController>(AccessTokenUI);
                AccessTokenUIController.Bind(token);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

}
