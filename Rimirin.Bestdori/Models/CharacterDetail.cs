using Rimirin.Bestdori.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rimirin.Bestdori.Models
{
    public class CharacterDetail
    {
        public string CharacterType { get; set; }
        public IList<string> CharacterName { get; set; }
        public IList<string> FirstName { get; set; }
        public IList<string> LastName { get; set; }
        public IList<string> Nickname { get; set; }
        public int BandId { get; set; }
        public string SdAssetBundleName { get; set; }
        public int DefaultCostumeId { get; set; }
        public IList<string> Ruby { get; set; }
        public Profile Profile { get; set; }
    }

    public class Profile
    {
        public IList<string> CharacterVoice { get; set; }
        public IList<string> FavoriteFood { get; set; }
        public IList<string> HatedFood { get; set; }
        public IList<string> Hobby { get; set; }
        public IList<string> SelfIntroduction { get; set; }
        public IList<string> School { get; set; }
        public IList<string> SchoolCls { get; set; }
        public IList<int> SchoolYear { get; set; }
        public string Part { get; set; }

        [JsonConverter(typeof(UnixTimeStringJsonConverter))]
        public DateTime? Birthday { get; set; }

        public string Constellation { get; set; }
        public int Height { get; set; }
    }
}