using System;
using System.Collections.Generic;

namespace Pckgs
{
    public class UnityPackage
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Name { get; set; }
        public List<UnityPackageRelease> Releases { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public List<UserRef> Contributors { get; set; }
    }
}