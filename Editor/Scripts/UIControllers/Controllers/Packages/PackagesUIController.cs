using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class PackagesUIController : UIController
    {
        public VisualElement Content { get; private set; }
        public VisualElement Loading { get; private set; }
        public TextField SearchField { get; private set; }
        public VisualElement Pagination { get; private set; }
        public VisualElement StartSearch { get; private set; }


        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                Content.style.display = _isLoading ? DisplayStyle.None : DisplayStyle.Flex;
                Loading.style.display = _isLoading ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private PckgsApi Api { get; set; } = new();

        private PaginatedDataUIController<UnityPackage, PackageUIController> paginatedDataUI;
        private CancellationTokenSource searchCancellationToken;
        public PackagesUIController(VisualElement target) : base(target)
        {
            Loading = target.Q<VisualElement>("Loading");
            Content = target.Q<VisualElement>("Content");
            SearchField = target.Q<TextField>("SearchField");
            Pagination = target.Q<VisualElement>("Pagination");
            StartSearch = target.Q<VisualElement>("StartSearch");

            SearchField.RegisterValueChangedCallback(e => Load(e.newValue));
            paginatedDataUI = CloneTree<PaginatedDataUIController<UnityPackage, PackageUIController>>(PckgsWindow.PaginatedDataUIAsset, Pagination);
            paginatedDataUI.Asset = PckgsWindow.PackageUIAsset;
            paginatedDataUI.OnPageChanged += (page) => Load(SearchField.text, page);

            Load();
        }

        public async void Load(string searchKey = null, int page = 0)
        {
            searchCancellationToken?.Cancel();
            searchCancellationToken = null;

            var isEmptySearch = string.IsNullOrEmpty(searchKey) || string.IsNullOrWhiteSpace(searchKey);
            StartSearch.style.display = isEmptySearch ? DisplayStyle.Flex : DisplayStyle.None;
            Pagination.style.display = isEmptySearch ? DisplayStyle.None : DisplayStyle.Flex;

            if (isEmptySearch) return;

            var token = new CancellationTokenSource();
            searchCancellationToken = token;

            IsLoading = true;
            try
            {
                await Task.Delay(500, token.Token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            var size = 25;
            var data = await Api.Packages.Search(searchKey, null, Mathf.Max(page - 1, 0) * size, size);
            if (token.IsCancellationRequested) return;

            IsLoading = false;
            paginatedDataUI.Bind(data);
        }
    }

}
