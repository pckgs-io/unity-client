using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class BoolDialogUIController : PopupUIController
    {
        public Label HeaderLabel { get; private set; }
        public Label DescriptionLabel { get; private set; }
        public VisualElement ActionSpacing { get; private set; }
        public Button NegativeButton { get; private set; }
        public Button PositiveButton { get; private set; }
        public VisualElement Content { get; private set; }

        public event Action<bool?> OnResult;

        public string Header
        {
            get => HeaderLabel.text;
            set => HeaderLabel.text = value;
        }
        public string Description
        {
            get => DescriptionLabel.text;
            set => DescriptionLabel.text = value;
        }
        public BoolDialogUIController(VisualElement target) : base(target)
        {
            HeaderLabel = target.Q<Label>("HeaderLabel");
            DescriptionLabel = target.Q<Label>("DescriptionLabel");
            ActionSpacing = target.Q<VisualElement>("ActionSpacing");
            NegativeButton = target.Q<Button>("NegativeButton");
            PositiveButton = target.Q<Button>("PositiveButton");
            Content = target.Q<VisualElement>("Content");

            NegativeButton.clicked += () =>
            {
                OnResult?.Invoke(false);
                OnResult = null;
                Close();
            };
            PositiveButton.clicked += () =>
            {
                OnResult?.Invoke(true);
                OnResult = null;
                Close();
            };
        }

        public override void Close()
        {
            OnResult?.Invoke(null);
            OnResult = null;
            base.Close();
        }
    }
}
