using UnityEngine.UIElements;

namespace Pckgs
{
    public class FileDataUIController : UIController, IBindable<FileData>
    {
        public FileDataUIController(VisualElement target) : base(target)
        {
        }
        public async void Bind(FileData obj)
        {
            Target.style.backgroundImage = null;
            Target.style.display = obj != null ? DisplayStyle.Flex : DisplayStyle.None;
            var texture = await PckgsApi.GetTextureFile(obj);
            Target.style.backgroundImage = texture;
        }
    }

}