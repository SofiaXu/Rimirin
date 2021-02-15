using Mirai_CSharp;
using Mirai_CSharp.Models;
using System.Threading.Tasks;

namespace Rimirin.Framework.Handlers.Interfaces
{
    /// <summary>
    /// 好友消息处理器
    /// </summary>
    public interface IFriendMessageHandler : IMessageHandler
    {
        /// <summary>
        /// 处理好友消息
        /// </summary>
        /// <param name="session">连接客户端</param>
        /// <param name="chain">消息链</param>
        /// <param name="info">好友信息</param>
        /// <returns></returns>
        public Task DoHandle(MiraiHttpSession session, IMessageBase[] chain, IFriendInfo info);
    }
}