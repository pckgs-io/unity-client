using System;

namespace Pckgs
{
    public class FileData
    {
        public string FileId { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string FileName { get; set; }
        public string Sha256Hash { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}