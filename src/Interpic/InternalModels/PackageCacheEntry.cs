using Interpic.Models.Packaging;
using System;

namespace Interpic.Studio.InternalModels
{
    public class PackageCacheEntry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Folder { get; set; }
        public string PackageFile { get; set; }
        public PackageManifest Manifest { get; set; }
    }
}
