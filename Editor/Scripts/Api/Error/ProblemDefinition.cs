using System.Collections.Generic;

namespace Pckgs
{
    public class ProblemDefinition
    {
        public int Status { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string TraceId { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
        public string Detail { get; set; }
    }
}