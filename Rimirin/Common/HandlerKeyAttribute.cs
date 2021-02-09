using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Common
{
    /// <summary>
    /// 本特性用于消息关键字，使用正则表达式来识别文中关键字
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class HandlerKeyAttribute : Attribute
    {
        public string Keyword { get; }
        /// <summary>
        /// 本特性用于消息关键字，使用正则表达式来识别文中关键字
        /// </summary>
        /// <param name="keyword">检测关键字所用的正则表达式</param>
        public HandlerKeyAttribute(string keyword)
        {
            Keyword = keyword;
        }
    }
}
