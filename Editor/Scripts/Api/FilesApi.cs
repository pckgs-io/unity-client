using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Pckgs
{
    public class FilesApi : BaseApi<AccessToken>
    {
        public FilesApi(string endPoint, string accessToken) : base(endPoint, accessToken)
        {
        }

        public async Task<Texture2D> GetTextureFile(FileData fileData)
        {
            if (fileData == null) return null;
            var request = UnityWebRequestTexture.GetTexture(EndPoint + $"/{fileData.FileId}");
            return await Request(request, req => DownloadHandlerTexture.GetContent(request));
        }

    }
}