using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Pckgs
{
    public class PckgsApi
    {
        public static string EndPoint => Navigation.Backend;

        public static async Task<UnityPackage> PublishPackage(string registrySlug, string metadata, byte[] tarball, string token)
        {
            var form = new WWWForm();
            form.AddField("metadata", metadata);
            form.AddBinaryData("packageFile", tarball, "package.tgz", "application/gzip");

            var request = UnityWebRequest.Post(EndPoint + $"/{registrySlug}", form);
            request.SetRequestHeader("Authorization", "Bearer: " + token);
            return await Request<UnityPackage>(request);
        }

        public static async Task<Texture2D> GetTextureFile(FileData fileData)
        {
            if (fileData == null) return null;
            var request = UnityWebRequestTexture.GetTexture(EndPoint + $"/files/{fileData.FileId}");
            return await Request(request, req => DownloadHandlerTexture.GetContent(request));
        }

        public static async Task<AccessTokenDetails> GetAccessTokenDetails(string token)
        {
            var form = new WWWForm();
            form.AddField("token", token);
            var details = await Request<AccessTokenDetails>(UnityWebRequest.Post(EndPoint + $"/access-tokens/details", form));
            details.AccessToken.Token = token;
            return details;
        }

        public static Task<T> Request<T>(UnityWebRequest request) => Request(request, (req) =>
        {
            var responseText = req.downloadHandler.text;
            return JsonConvert.DeserializeObject<T>(responseText);
        });
        public static Task<T> Request<T>(UnityWebRequest request, Func<UnityWebRequest, T> extractor)
        {
            var completion = new TaskCompletionSource<T>();

            var operation = request.SendWebRequest();
            operation.completed += (_) =>
            {
                try
                {
                    if (request.result == UnityWebRequest.Result.Success)
                        completion.SetResult(extractor(request));
                    else if (request.result == UnityWebRequest.Result.ConnectionError)
                        throw new ApiConnectionException("Unable to connect to pckgs.io servers, make sure you have internet connection");
                    else
                    {
                        var responseText = request.downloadHandler.text;
                        var problem = JsonConvert.DeserializeObject<ProblemDefinition>(responseText);
                        throw new HttpException(problem);
                    }
                }
                catch (Exception e)
                {
                    completion.SetException(e);
                }
            };

            return completion.Task;
        }
    }
}
