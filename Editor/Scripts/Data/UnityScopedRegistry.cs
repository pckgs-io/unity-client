using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Pckgs
{
    public class UnityScopedRegistry
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public List<string> Scopes { get; set; }      
    }
}