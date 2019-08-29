using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.InternalModels
{
    public class PackageCache
    {
        public DateTime CacheDate { get; set; }
        public List<PackageCacheEntry> Entries { get; set; }
    }
}
