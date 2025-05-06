using System;
using System.Collections.Generic;

namespace Pckgs
{
    public class AccessToken
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public string OrganizationId { get; set; }
        public Dictionary<string, byte> Scopes { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
