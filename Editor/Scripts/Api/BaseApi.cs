using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Pckgs
{
    public abstract class BaseApi<T>
    {
        public string EndPoint { get; private set; }
        public string AccessToken { get; private set; }

        public BaseApi(string endPoint, string accessToken)
        {
            EndPoint = endPoint;
            AccessToken = accessToken;
        }

        public Task<T> Get(string id) => Request<T>(UnityWebRequest.Get(EndPoint + $"/{id}"));
        public Task<IEnumerable<T>> GetAll() => Request<IEnumerable<T>>(UnityWebRequest.Get(EndPoint));

        protected Task<N> Request<N>(UnityWebRequest request) => Request(request, (req) =>
        {
            var responseText = req.downloadHandler.text;
            return JsonConvert.DeserializeObject<N>(responseText);
        });
        protected Task<N> Request<N>(UnityWebRequest request, Func<UnityWebRequest, N> extractor)
        {
            var completion = new TaskCompletionSource<N>();

            if (!string.IsNullOrEmpty(AccessToken))
                request.SetRequestHeader("Authorization", "Bearer " + AccessToken);

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
