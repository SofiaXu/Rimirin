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
        public string HelpText { get; }

        /// <summary>
        /// 处理器模块名
        /// </summary>
        public string HandlerName { get; }

        /// <summary>
        /// 用于表示定时处理器的时间间隔的特性。
        /// </summary>
        /// <param name="interval">执行的时间间隔</param>
        /// <param name="helpText">帮助文本</param>
        /// <param name="handlerName">处理器模块名（在帮助中显示的模块名称，同一个处理器处理不同指令建议使用同一个助记名）</param>
        public TimeHandlerAttribute(TimeSpan interval, string helpText = null, string handlerName = null)
        {
            Interval = interval;
            HelpText = helpText;
            HandlerName = handlerName;
        }
    }
}
