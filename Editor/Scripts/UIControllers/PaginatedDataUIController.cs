using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class PaginatedDataUIController<T, TController> : UIController, IBindable<PaginatedData<T>> where TController : UIController, IBindable<T>
    {
        public VisualTreeAsset Asset { get; set; }

        public VisualElement NoContent { get; private set; }
        public VisualElement Content { get; private set; }
        public Label CountLabel { get; private set; }
        public ScrollView List { get; private set; }
        public VisualElement Pagination { get; private set; }

        public event Action<int> OnPageChanged;

        public PaginatedDataUIController(VisualElement target) : base(target)
        {
            NoContent = target.Q<VisualElement>("NoContent");
            Content = target.Q<VisualElement>("Content");
            CountLabel = target.Q<Label>("CountLabel");
            List = target.Q<ScrollView>("List");
            Pagination = target.Q<VisualElement>("Pagination");
        }

        public void Bind(PaginatedData<T> obj)
        {
            ReleaseChildren();
            Pagination.Clear();

            var hasContent = obj.Size > 0;

            NoContent.style.display = hasContent ? DisplayStyle.None : DisplayStyle.Flex;
            Content.style.display = hasContent ? DisplayStyle.Flex : DisplayStyle.None;

            if (!hasContent) return;

            CountLabel.style.display = obj.Total > obj.Size ? DisplayStyle.Flex : DisplayStyle.None;
            CountLabel.text = $"Showing results <b>{obj.From + 1}-{obj.From + obj.Size}</b> out of <b>{obj.Total}</b>";

            Pagination.style.display = obj.Total > obj.PerPage ? DisplayStyle.Flex : DisplayStyle.None;

            foreach (var data in obj.Data)
            {
                var controller = CloneTree<TController>(Asset, List);
                controller.Bind(data);
            }

            var pages = obj.GetPaginationPages();
            var currentPage = obj.GetCurrentPage();

            foreach (var pageItem in pages)
            {
                var isNumber = int.TryParse(pageItem, out var pageNumber);
                var label = new Label();
                label.AddToClassList("p");
                label.text = pageItem;
                if (isNumber)
                {
                    label.AddToClassList("link");
                    if (pageNumber == currentPage)
                        label.text = $"<u>{currentPage}</u>";

                    label.RegisterCallback<ClickEvent>(e =>
                    {
                        if (e.target == label)
                            OnPageChanged?.Invoke(pageNumber);
                    });
                }
                Pagination.Add(label);
            }

        }
    }

}