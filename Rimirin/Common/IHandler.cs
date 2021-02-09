using Mirai_CSharp;
using Mirai_CSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Common
{
    /// <summary>
    /// 处理器
    /// </summary>
    public interface IHandler
    {
    }
    /// <summary>
    /// 消息处理器
    /// </summary>
    public interface IMessageHandler : IHandler
    {
        /// <summary>
        /// 对发送来的消息链进行处理
        /// </summary>
        /// <param name="session">mirai连接</param>
        /// <param name="chain">消息链</param>
        /// <param name="info">发送者信息</param>
        void DoHandle(MiraiHttpSession session, IMessageBase[] chain, IBaseInfo info);
    }
    /// <summary>
    /// 群消息处理器
    /// </summary>
    public interface IGroupMessageHandler : IMessageHandler
    {
    }
    /// <summary>
    /// 好友消息处理器
    /// </summary>
    public interface IFriendMessageHandler : IMessageHandler
    {
    }
}
