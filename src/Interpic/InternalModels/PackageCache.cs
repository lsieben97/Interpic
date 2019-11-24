using System;
using System.Collections.Generic;

namespace Interpic.Studio.InternalModels
{
    public class PackageCache
    {
        public DateTime CacheDate { get; set; }
        public List<PackageCacheEntry> Entries { get; set; }
    }
}
