using System;

namespace Rimirin.Framework.Handlers.Announces
{
    /// <summary>
    /// 用于表示消息处理器的命令关键字的特性，使用正则表达式来识别文中关键字。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MessageHandlerAttribute : Attribute
    {
        /// <summary>
        /// 正则表达式
        /// </summary>
        public string Regex { get; }

        /// <summary>
        /// 帮助文本
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// 助记名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 用于表示消息处理器的命令关键字的特性，使用正则表达式来识别文中关键字。
        /// </summary>
        /// <param name="regex">识别用正则表达式</param>
        /// <param name="helpText">帮助文本</param>
        /// <param name="alias">助记名</param>
        public MessageHandlerAttribute(string regex, string helpText = null, string alias = null)
        {
            Regex = regex;
            HelpText = helpText;
            Alias = alias;
        }
    }
}