using Mirai_CSharp;
using Mirai_CSharp.Models;
using System.Threading.Tasks;

namespace Rimirin.Framework.Handlers.Interfaces
{
    /// <summary>
    /// 临时消息处理器
    /// </summary>
    public interface ITempMessageHandler : IMessageHandler
    {
        public Task DoHandle(MiraiHttpSession session, IMessageBase[] chain, IGroupMemberInfo info);
    }
}