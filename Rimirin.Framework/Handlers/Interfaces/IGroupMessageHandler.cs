using Mirai_CSharp;
using Mirai_CSharp.Models;
using System.Threading.Tasks;

namespace Rimirin.Framework.Handlers.Interfaces
{
    /// <summary>
    /// 群消息处理器
    /// </summary>
    public interface IGroupMessageHandler : IMessageHandler
    {
        /// <summary>
        /// 处理群消息
        /// </summary>
        /// <param name="session">连接客户端</param>
        /// <param name="chain">消息链</param>
        /// <param name="info">群信息</param>
        /// <returns></returns>
        public Task DoHandle(MiraiHttpSession session, IMessageBase[] chain, IGroupMemberInfo info);
    }
}