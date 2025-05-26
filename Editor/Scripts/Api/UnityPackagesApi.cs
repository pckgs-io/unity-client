using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Pckgs
{
    public class UnityPackagesApi : BaseApi<UnityPackage>
    {
        public UnityPackagesApi(string endPoint, string accessToken) : base(endPoint, accessToken)
        {
        }

        public async Task<UnityPackage> PublishPackage(string metadata, byte[] tarball)
        {
            var form = new WWWForm();
            form.AddField("metadata", metadata);
            form.AddBinaryData("packageFile", tarball, "package.tgz", "application/gzip");

            var request = UnityWebRequest.Post(EndPoint, form);
            return await Request<UnityPackage>(request);
        }

        public async Task<PaginatedData<UnityPackage>> Search(string searchKeyword, string organizationId, int? from, int? size)
        {
            var url = new StringBuilder();
            url.Append(EndPoint);
            url.Append("/search?");

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                url.Append("&searchKey=");
                url.Append(searchKeyword);
            }
            if (!string.IsNullOrEmpty(organizationId))
            {
                url.Append("&organizationId=");
                url.Append(organizationId);
            }
            if (from.HasValue)
            {
                url.Append("&from=");
                url.Append(from.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (size.HasValue)
            {
                url.Append("&size=");
                url.Append(size.Value.ToString(CultureInfo.InvariantCulture));
            }

            var request = UnityWebRequest.Get(url.ToString());
            return await Request<PaginatedData<UnityPackage>>(request);
        }
    }
}