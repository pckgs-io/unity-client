using System;
using System.Collections.Generic;

namespace Pckgs
{
    public class AccessTokenDetails
    {
        public AccessToken AccessToken { get; set; }

        public Dictionary<string, AccessTokenOrganizationDetail> Organizations { get; set; }
        public string UserEmail { get; set; }
    }
}
