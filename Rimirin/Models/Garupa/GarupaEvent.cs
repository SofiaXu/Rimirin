using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
