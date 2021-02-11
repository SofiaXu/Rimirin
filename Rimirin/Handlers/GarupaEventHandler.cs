using Microsoft.Extensions.Logging;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Rimirin.Common;
using Rimirin.Garupa;
using System;

namespace Rimirin.Handlers
{
    [HandlerKey("^最新活动$")]
    public class GarupaEventHandler : IHandler, IMessageHandler, IGroupMessageHandler
    {
        private readonly GarupaData data;
        private readonly BestdoriClient client;
        private readonly ILogger<GarupaEventHandler> logger;
        private int latestEventId;

        public GarupaEventHandler(GarupaData data, BestdoriClient client, ILogger<GarupaEventHandler> logger)
        {
            this.data = data;
            this.client = client;
            this.logger = logger;
            latestEventId = -1;
        }

        public async void DoHandleAsync(MiraiHttpSession session, IMessageBase[] chain, IBaseInfo info, bool isGroupMessage = true)
        {
            logger.LogInformation($"{info.Name} 询问最新活动");
            var latestEvent = data.GetLatestEvent();
            bool isEventChanged = false;
            if (int.Parse(latestEvent.Item1) != latestEventId)
            {
                latestEventId = int.Parse(latestEvent.Item1);
                isEventChanged = true;
            }
            string message;
            IMessageBase img;
            if (isGroupMessage)
                img = await session.UploadPictureAsync(UploadTarget.Group, await client.GetEventBannerImagePath(latestEvent.Item1));
            else
                img = await session.UploadPictureAsync(UploadTarget.Friend, await client.GetEventBannerImagePath(latestEvent.Item1));
            DateTime endTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(latestEvent.Item2.EndAt[0])).LocalDateTime;
            DateTime startTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(latestEvent.Item2.StartAt[0])).LocalDateTime;
            string eventState = startTime >= DateTime.Now ? $"即将开始，剩余{startTime - DateTime.Now:d\\日h\\小\\时m\\分}" : endTime <= DateTime.Now ? "结束" : $"正在进行，剩余{endTime - DateTime.Now:d\\日h\\小\\时m\\分}";
            message = $"活动名称: {latestEvent.Item2.EventName[0]}\n开始时间: {startTime}\n结束时间: {endTime}\n活动状态: {eventState}";
            logger.LogInformation($"已查到活动，并回复 {message}");
            IMessageBase[] result = new IMessageBase[]
            {
                    new PlainMessage((isEventChanged ? "发现新活动！\n" : "") + message),
                    img
            };
            if (isGroupMessage)
            {
                await session.SendGroupMessageAsync(((IGroupMemberInfo)info).Group.Id, result);
            }
            else
            {
                await session.SendFriendMessageAsync(info.Id, result);
            }
        }
    }
}