using System;
using System.Collections.Generic;

namespace Rimirin.Bestdori.Models
{
    public class GarupaEvent
    {
        public string EventType { get; set; }
        public IList<string> EventName { get; set; }
        public string BannerAssetBundleName { get; set; }
        public IList<DateTime?> StartAt { get; set; }
        public IList<DateTime?> EndAt { get; set; }
        public IList<int> RewardCards { get; set; }
    }
}