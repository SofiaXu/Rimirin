using System.Collections.Generic;

namespace Rimirin.Models.Garupa
{
    public class GarupaEvent
    {
        public string EventType { get; set; }
        public IList<string> EventName { get; set; }
        public string BannerAssetBundleName { get; set; }
        public IList<string> StartAt { get; set; }
        public IList<string> EndAt { get; set; }
        public IList<int> RewardCards { get; set; }
    }
}