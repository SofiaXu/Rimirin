using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Models.Garupa
{
    public class GachaDetail
    {
        public Dictionary<string, Detail> Details { get; set; }
        public Dictionary<string, StarRate> Rates { get; set; }
        public Paymentmethod[] PaymentMethods { get; set; }
        public string ResourceName { get; set; }
        public string BannerAssetBundleName { get; set; }
        public string[] GachaName { get; set; }
        public string[] PublishedAt { get; set; }
        public string[] closedAt { get; set; }
        public string[] Description { get; set; }
        public string[] Annotation { get; set; }
        public string[] GachaPeriod { get; set; }
        public string GachaType { get; set; }
        public string Type { get; set; }
        public int[] NewCards { get; set; }
        public Information Information { get; set; }
    }

    public class Information
    {
        public string[] description { get; set; }
        public string[] term { get; set; }
        public string[] newMemberInfo { get; set; }
        public string[] notice { get; set; }
    }

    public class Detail
    {
        public int rarityIndex { get; set; }
        public int weight { get; set; }
        public bool pickup { get; set; }
    }

    public class StarRate
    {
        public int Rate { get; set; }
        public int WeightTotal { get; set; }
    }

    public class Paymentmethod
    {
        public int GachaId { get; set; }
        public string PaymentMethod { get; set; }
        public int Quantity { get; set; }
        public int PaymentMethodId { get; set; }
        public int Count { get; set; }
        public string Behavior { get; set; }
        public bool Pickup { get; set; }
        public int CostItemQuantity { get; set; }
    }

}
