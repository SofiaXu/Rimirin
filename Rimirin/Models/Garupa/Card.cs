namespace Rimirin.Models.Garupa
{
    public class Card
    {
        public int CharacterId { get; set; }
        public string Attribute { get; set; }
        public string[] Prefix { get; set; }
        public string ResourceSetName { get; set; }
        public int Rarity { get; set; }
    }
}