using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Models.Garupa
{
    class Gacha
    {
        public string ResourceName { get; set; }
        public string BannerAssetBundleName { get; set; }
        public IList<string> GachaName { get; set; }
        public IList<string> PublishedAt { get; set; }
        public IList<string> ClosedAt { get; set; }
        public string Type { get; set; }
        public IList<int> NewCards { get; set; }
    }
}
