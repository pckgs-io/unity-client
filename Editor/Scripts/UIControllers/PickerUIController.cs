using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class PickerUIController : PopupUIController
    {
        public Label HeaderLabel { get; private set; }
        public ScrollView OptionList { get; private set; }

        private Action<object> ActiveCallback;
        public string Header
        {
            get => HeaderLabel.text;
            set => HeaderLabel.text = value;
        }

        public PickerUIController(VisualElement target) : base(target)
        {
            HeaderLabel = target.Q<Label>("HeaderLabel");
            OptionList = target.Q<ScrollView>("Options");
        }

        public static PickerUIController Select(IEnumerable<IPickerOption> options, Action<object> callback)
        {
            var controller = PckgsWindow.ShowPopup<PickerUIController>(PckgsWindow.PickerUIAsset);
            controller.DoSelect(options, callback);
            return controller;
        }

        public override void Close()
        {
            ActiveCallback?.Invoke(null);
            ActiveCallback = null;
            base.Close();
        }

        private void DoSelect(IEnumerable<IPickerOption> options, Action<object> callback)
        {
            ActiveCallback = callback;
            foreach (var option in options)
            {
                var ui = PckgsWindow.PickerOptionUIAsset.Instantiate();
                OptionList.Add(ui);

                var controller = new PickerOptionUIController(ui);
                controller.Bind(option);
                controller.OnSelected += (o) =>
                {
                    callback?.Invoke(o.Data);
                    Close();
                };
            }
        }
    }
}
