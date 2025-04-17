using System;
using UnityEngine.UIElements;

namespace Pckgs
{
    public abstract class UIController : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public VisualElement Target { get; private set; }
        public UIController(VisualElement target)
        {
            Target = target;
        }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            OnDisposed();
        }

        protected virtual void OnDisposed()
        {

        }
    }
}