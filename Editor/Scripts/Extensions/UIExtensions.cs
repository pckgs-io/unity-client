using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public static class UIExtensions
    {
        public static T CloneTree<T>(this VisualTreeAsset asset, VisualElement target) where T : UIController
        {
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            if (target == null) throw new ArgumentNullException(nameof(target));

            var ui = asset.Instantiate();
            ui.pickingMode = PickingMode.Ignore;
            ui.style.alignItems = Align.Stretch;
            ui.style.justifyContent = Justify.Center;
            target.Add(ui);

            /*asset.CloneTree(target.contentContainer, out var index, out var count);

            var elements = new VisualElement[count];
            for (int i = 0; i < count; i++)
                elements[i] = target.ElementAt(index + i);
*/
            return (T)Activator.CreateInstance(typeof(T), ui);
        }
    }
}
