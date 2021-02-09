using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Options
{
    public class MiraiSessionOptions
    {
        public const string MiraiSession = "MiraiSession";

        /// <summary>
        /// Mirai主机地址
        /// </summary>
        public string MiraiHost { get; set; }
        /// <summary>
        /// Mirai主机端口
        /// </summary>
        public int MiraiHostPort { get; set; }
        /// <summary>
        /// Mirai连接QQ
        /// </summary>
        public long MiraiSessionQQ { get; set; }
        /// <summary>
        /// Mirai authKey
        /// </summary>
        public string MiraiSessionKey { get; set; }
    }
}
