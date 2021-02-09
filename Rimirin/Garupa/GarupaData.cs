using Rimirin.Models.Garupa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 获取最后一次活动
        /// </summary>
        /// <returns>活动Id，活动信息</returns>
        public (string, GarupaEvent) GetLatestEvent()
        {
            string key = Events.Keys.Max(k => int.Parse(k)).ToString();
            return (key, Events[Events.Keys.Max(k => int.Parse(k)).ToString()]);
        }
    }
}
