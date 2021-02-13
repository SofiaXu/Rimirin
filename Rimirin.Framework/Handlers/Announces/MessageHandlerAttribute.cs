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
        public string HelpText { get; }

        /// <summary>
        /// 处理器模块名
        /// </summary>
        public string HandlerName { get; }

        /// <summary>
        /// 帮助中显示命令
        /// </summary>
        public string HelpCommand { get; }

        /// <summary>
        /// 用于表示消息处理器的命令关键字的特性，使用正则表达式来识别文中关键字。
        /// </summary>
        /// <param name="regex">识别用正则表达式</param>
        /// <param name="helpCommand">帮助中显示命令</param>
        /// <param name="helpText">帮助文本</param>
        /// <param name="handlerName">处理器模块名（在帮助中显示的模块名称，同一个处理器处理不同指令建议使用同一个助记名）</param>
        public MessageHandlerAttribute(string regex, string handlerName = null, string helpCommand = null, string helpText = null)
        {
            Regex = regex;
            HelpText = helpText;
            HandlerName = handlerName;
            HelpCommand = helpCommand;
        }
    }
}