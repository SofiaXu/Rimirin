using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Framework.Handlers.Announces
{
    /// <summary>
    /// 用于表示定时处理器的时间间隔的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TimeHandlerAttribute : Attribute
    {
        /// <summary>
        /// 时间间隔
        /// </summary>
        public TimeSpan Interval { get; }

        /// <summary>
        /// 帮助文本
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// 助记名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 用于表示定时处理器的时间间隔的特性。
        /// </summary>
        /// <param name="regex">识别用正则表达式</param>
        /// <param name="helpText">帮助文本</param>
        /// <param name="alias">助记名</param>
        public TimeHandlerAttribute(TimeSpan interval, string helpText = null, string alias = null)
        {
            Interval = interval;
            HelpText = helpText;
            Alias = alias;
        }
    }
}
