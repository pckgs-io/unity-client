using System;
using UnityEngine.UIElements;

namespace Pckgs
{
    public abstract class PopupUIController : UIController
    {
        public event Action OnClosed;
        protected PopupUIController(VisualElement target) : base(target)
        {
        }

        public virtual void Close()
        {
            Dispose();
            OnClosed?.Invoke();
        }
    }
}
