using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Pckgs
{
    public class AccessTokenApi : BaseApi<AccessToken>
    {
        public AccessTokenApi(string endPoint, string accessToken) : base(endPoint, accessToken)
        {
        }

        public async Task<AccessTokenDetails> GetAccessTokenDetails(string token)
        {
            var form = new WWWForm();
            form.AddField("token", token);
            var details = await Request<AccessTokenDetails>(UnityWebRequest.Post(EndPoint + "/details", form));
            details.AccessToken.Token = token;
            return details;
        }

    }
}