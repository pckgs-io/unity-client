using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Pckgs
{
    public static class UIExtensions
    {
        public static T CloneTree<T>(this VisualTreeAsset asset, VisualElement target) where T : UIController
        {
            asset.CloneTree(target.contentContainer);
            return (T)Activator.CreateInstance(typeof(T), target);
        }
    }
}
