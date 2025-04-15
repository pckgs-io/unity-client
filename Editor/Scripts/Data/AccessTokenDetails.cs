using System;

namespace Pckgs
{
    public class AccessTokenDetails
    {
        public AccessToken AccessToken { get; set; }

        public string OrganizationName { get; set; }
        public string OrganizationSlug { get; set; }
        public FileData OrganizationLogo { get; set; }
        public string UserEmail { get; set; }
    }
}
