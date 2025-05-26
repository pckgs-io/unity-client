using System;
using System.Collections.Generic;

namespace Pckgs
{
    public class AccessToken
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
        public List<AccessTokenOrganizationAccess> Accesses { get; set; }
        public string Slug { get; set; }
        public string UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
