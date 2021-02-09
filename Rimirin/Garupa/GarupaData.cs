using Rimirin.Models.Garupa;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rimirin.Garupa
{
    public class GarupaData
    {
        /// <summary>
        /// 活动列表
        /// </summary>
        public Dictionary<string, GarupaEvent> Events { get; set; }

        /// <summary>
        /// 角色列表
        /// </summary>
        public Dictionary<string, Character> Characters { get; set; }

        /// <summary>
        /// 卡片列表
        /// </summary>
        public Dictionary<string, Card> Cards { get; set; }

        /// <summary>
        /// 乐队列表
        /// </summary>
        public Dictionary<string, Band> Bands { get; set; }

        /// <summary>
        /// 卡池列表
        /// </summary>
        public Dictionary<string, Gacha> Gacha { get; set; }

        /// <summary>
        /// 当期卡池详细信息列表
        /// </summary>
        public Dictionary<string, GachaDetail> RecentGachaDetails { get; set; }

        /// <summary>
        /// 获取最后一次活动
        /// </summary>
        /// <returns>活动Id，活动信息</returns>
        public (string, GarupaEvent) GetLatestEvent()
        {
            string key = Events.Keys.Max(k => int.Parse(k)).ToString();
            return (key, Events[Events.Keys.Max(k => int.Parse(k)).ToString()]);
        }

        public Dictionary<string, Gacha> GetRecentGacha()
        {
            return Gacha.Where(k => !string.IsNullOrEmpty(k.Value.ClosedAt[0]) && DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(k.Value.ClosedAt[0])).LocalDateTime >= DateTime.Now).ToDictionary(k => k.Key, k => k.Value);
        }
    }
}