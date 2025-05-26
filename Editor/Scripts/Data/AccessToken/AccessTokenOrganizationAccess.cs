using System.Collections.Generic;

namespace Pckgs
{
    public class AccessTokenOrganizationAccess
    {
        public string OrganizationId { get; set; }
        public bool AllowCreatePackage { get; set; }
        public List<AccessTokenPackageAccess> PackageAccesses { get; set; }

    }
}
