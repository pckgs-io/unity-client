using UnityEngine.UIElements;

namespace Pckgs
{
    public abstract class UIController
    {
        public VisualElement Target { get; private set; }
        public UIController(VisualElement target)
        {
            Target = target;
        }
    }
}