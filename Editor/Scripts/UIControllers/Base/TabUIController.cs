using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{

    public abstract class TabUIController : UIController
    {
        public TabView TabView { get; private set; }
        protected UIController ActiveUIController { get; private set; }

        protected TabUIController(VisualElement target) : base(target)
        {
            TabView = target.Q<TabView>("TabView");
        }

        protected void ActivateTab<T>(VisualTreeAsset asset, Tab tab) where T : UIController
        {
            ActiveUIController?.Dispose();
            ActiveUIController = asset.CloneTree<T>(tab);
        }
        protected override void OnDisposed()
        {
            base.OnDisposed();
            ActiveUIController?.Dispose();
            ActiveUIController = null;
        }
    }
}

