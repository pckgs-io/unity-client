using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{

    public interface IPickerOption
    {
        public Task<Texture2D> GetImage();
        public Task<string> GetName();
        public object Data { get; }
    }

    public class AsyncPickerOption : IPickerOption
    {
        private Func<Task<Texture2D>> ImageGetter { get; set; }
        private Func<Task<string>> NameGetter { get; set; }
        public object Data { get; private set; }

        public AsyncPickerOption(object data, Func<Task<Texture2D>> imageGetter, Func<Task<string>> nameGetter)
        {
            Data = data;
            ImageGetter = imageGetter;
            NameGetter = nameGetter;
        }

        public Task<Texture2D> GetImage() => ImageGetter();
        public Task<string> GetName() => NameGetter();
    }


    public class PickerOptionUIController : UIController, IBindable<IPickerOption>
    {
        public event Action<IPickerOption> OnSelected;

        public VisualElement Container { get; private set; }
        public VisualElement IconContainer { get; private set; }
        public VisualElement IconElement { get; private set; }
        public Spinner IconSpinner { get; private set; }
        public Label NameLabel { get; private set; }
        public Spinner NameSpinner { get; private set; }

        public bool IsImageLoaded { get; private set; }
        public bool IsNameLoaded { get; private set; }

        public IPickerOption Option { get; private set; }

        public PickerOptionUIController(VisualElement target) : base(target)
        {
            Container = target.Q<VisualElement>("Container");
            IconContainer = target.Q<VisualElement>("IconContainer");
            IconElement = target.Q<VisualElement>("IconElement");
            IconSpinner = target.Q<Spinner>("IconSpinner");
            NameLabel = target.Q<Label>("NameLabel");
            NameSpinner = target.Q<Spinner>("NameSpinner");

            IconSpinner.style.display = DisplayStyle.None;
            NameSpinner.style.display = DisplayStyle.None;
            IconElement.style.backgroundImage = null;
            NameLabel.text = null;

            IconContainer.RegisterCallback<GeometryChangedEvent>(e =>
            {
                IconContainer.style.width = IconContainer.resolvedStyle.height;
            });
            Container.RegisterCallback<ClickEvent>(e =>
            {
                if (IsNameLoaded && IsImageLoaded)
                    OnSelected?.Invoke(Option);
            });
        }

        public void Bind(IPickerOption option)
        {
            Option = option;
            SetIcon(option);
            SetName(option);
        }

        async void SetIcon(IPickerOption option)
        {
            try
            {
                IsImageLoaded = false;
                IconSpinner.style.display = DisplayStyle.Flex;
                IconElement.style.display = DisplayStyle.None;

                var texture = await option.GetImage();
                IconElement.style.backgroundImage = texture;
            }
            finally
            {
                IsImageLoaded = true;
                IconSpinner.style.display = DisplayStyle.None;
                IconElement.style.display = DisplayStyle.Flex;
            }
        }
        async void SetName(IPickerOption option)
        {
            try
            {
                IsNameLoaded = false;
                NameSpinner.style.display = DisplayStyle.Flex;
                NameLabel.style.display = DisplayStyle.None;
                var name = await option.GetName();
                NameLabel.text = name;
            }
            finally
            {
                IsNameLoaded = true;
                NameSpinner.style.display = DisplayStyle.None;
                NameLabel.style.display = DisplayStyle.Flex;
            }
        }
    }
}
