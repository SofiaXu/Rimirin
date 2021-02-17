using System;
using System.Collections.Generic;

namespace Rimirin.Bestdori.Models
{
    public class Gacha
    {
        public string ResourceName { get; set; }
        public string BannerAssetBundleName { get; set; }
        public IList<string> GachaName { get; set; }
        public IList<DateTime?> PublishedAt { get; set; }
        public IList<DateTime?> ClosedAt { get; set; }
        public string Type { get; set; }
        public IList<int> NewCards { get; set; }
    }
}