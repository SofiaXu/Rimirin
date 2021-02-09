using System.Collections.Generic;

namespace Rimirin.Models.Garupa
{
    public class GachaDetail
    {
        public Dictionary<string, Detail>[] Details { get; set; }
        public Dictionary<string, StarRate>[] Rates { get; set; }
        public Paymentmethod[] PaymentMethods { get; set; }
        public string ResourceName { get; set; }
        public string BannerAssetBundleName { get; set; }
        public string[] GachaName { get; set; }
        public string[] PublishedAt { get; set; }
        public string[] ClosedAt { get; set; }
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
        public string[] Description { get; set; }
        public string[] Term { get; set; }
        public string[] NewMemberInfo { get; set; }
        public string[] Notice { get; set; }
    }

    public class Detail
    {
        public int RarityIndex { get; set; }
        public int Weight { get; set; }
        public bool Pickup { get; set; }
    }

    public class StarRate
    {
        public double Rate { get; set; }
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