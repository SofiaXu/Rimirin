using Mirai_CSharp;
using Mirai_CSharp.Models;
using Rimirin.Common;
using Rimirin.Garupa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Handlers
{
    [HandlerKey("最新活动")]
    public class EventHandler : IHandler, IMessageHandler, IGroupMessageHandler, IFriendMessageHandler
    {
        private readonly GarupaData data;
        private readonly BestdoriClient client;
        public EventHandler(GarupaData data, BestdoriClient client)
        {
            this.data = data;
            this.client = client;
        }

        public async void DoHandle(MiraiHttpSession session, IMessageBase[] chain, IBaseInfo info)
        {
            var latestEvent = data.GetLatestEvent();
            var img = await session.UploadPictureAsync(UploadTarget.Friend, await client.GetEventBannerImagePath(latestEvent.Item1));
            DateTime endTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(latestEvent.Item2.EndAt[0])).LocalDateTime;
            DateTime startTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(latestEvent.Item2.StartAt[0])).LocalDateTime;
            string eventState = startTime <= DateTime.Now ? $"即将开始，剩余{startTime - DateTime.Now:d\\日h\\小\\时m\\分}" : endTime <= DateTime.Now ? "结束" : $"正在进行，剩余{endTime - DateTime.Now:d\\日h\\小\\时m\\分}";
            IMessageBase[] result = new IMessageBase[]
            {
                    new PlainMessage($"活动名称: {latestEvent.Item2.EventName[0]}\n开始时间: {startTime}\n结束时间: {endTime}\n活动状态: {eventState}"),
                    img
            };
            await session.SendFriendMessageAsync(info.Id, result);
        }
    }
}
