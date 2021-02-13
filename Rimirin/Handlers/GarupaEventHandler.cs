using Microsoft.Extensions.Logging;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Rimirin.Framework.Handlers.Announces;
using Rimirin.Framework.Handlers.Interfaces;
using Rimirin.Garupa;
using System;
using System.Threading.Tasks;

namespace Rimirin.Handlers
{
    [MessageHandler("^最新活动$", "邦邦活动", "最新活动", "查询最新的活动信息")]
    public class GarupaEventHandler : IHandler, IMessageHandler, IGroupMessageHandler
    {
        private readonly GarupaData data;
        private readonly BestdoriClient client;
        private readonly ILogger<GarupaEventHandler> logger;

        public GarupaEventHandler(GarupaData data, BestdoriClient client, ILogger<GarupaEventHandler> logger)
        {
            this.data = data;
            this.client = client;
            this.logger = logger;
        }

        public async Task DoHandle(MiraiHttpSession session, IMessageBase[] chain, IGroupMemberInfo info)
        {
            logger.LogInformation($"{info.Name} 询问最新活动");
            var latestEvent = data.GetLatestEvent();
            bool isEventChanged = false;
            string message;
            IMessageBase img;
            img = await session.UploadPictureAsync(UploadTarget.Group, await client.GetEventBannerImagePath(latestEvent.Item1));
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
            await session.SendGroupMessageAsync(info.Group.Id, result);
        }
    }
}