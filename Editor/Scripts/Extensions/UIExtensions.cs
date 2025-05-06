using System;
using UnityEngine.UIElements;

namespace Pckgs
{
    public static class UIExtensions
    {
        public static T CloneTree<T>(this VisualTreeAsset asset, VisualElement target) where T : UIController
        {
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            if (target == null) throw new ArgumentNullException(nameof(target));

            asset.CloneTree(target.contentContainer);
            return (T)Activator.CreateInstance(typeof(T), target);
        }
    }
}
