using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Pckgs
{
    public abstract class UIController : IDisposable
    {
        protected virtual bool Expand { get; } = true;
        public bool IsDisposed { get; private set; }
        public VisualElement Target { get; private set; }

        private readonly List<UIController> children = new();

        public UIController(VisualElement target)
        {
            Target = target;
            if (Expand) target.style.flexGrow = 1f;
        }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            OnDisposed();

            if (Target != null && Target.parent != null)
            {
                Target.parent.Remove(Target);
            }
        }

        public T CloneTree<T>(VisualTreeAsset asset, VisualElement target) where T : UIController
        {
            var controller = asset.CloneTree<T>(target);
            children.Add(controller);
            return controller;
        }

        protected void ReleaseChildren()
        {
            foreach (var child in children)
                child.Dispose();
            children.Clear();
        }

        protected virtual void OnDisposed()
        {
            ReleaseChildren();
        }
    }
}