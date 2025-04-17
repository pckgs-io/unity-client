using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pckgs
{
    public class WindowUIController : PopupUIController
    {
        public Tab AccessTab { get; private set; }
        public Tab UploadTab { get; private set; }

        private AccessUIController accessUIController;
        private UploadUIController uploadUIController;
        public WindowUIController(VisualElement target) : base(target)
        {
            AccessTab = target.Q<Tab>("AccessTab");
            UploadTab = target.Q<Tab>("UploadTab");

            accessUIController = PckgsWindow.AccessUIAsset.CloneTree<AccessUIController>(AccessTab);
            uploadUIController = PckgsWindow.UploadUIAsset.CloneTree<UploadUIController>(UploadTab);
        }
        protected override void OnDisposed()
        {
            base.OnDisposed();
            accessUIController?.Dispose();
            uploadUIController?.Dispose();
        }
    }
}
