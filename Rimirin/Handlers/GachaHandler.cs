﻿using Microsoft.Extensions.Logging;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Rimirin.Common;
using Rimirin.Garupa;
using Rimirin.Models.Garupa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Handlers
{
    [HandlerKey("^十连$")]
    [HandlerKey("^单抽$")]
    [HandlerKey("^当期池子$")]
    public class GachaHandler : IHandler, IMessageHandler, IGroupMessageHandler
    {
        private readonly GarupaData data;
        private readonly BestdoriClient client;
        private readonly Random randomer = new Random();
        private readonly Dictionary<int, string> stars;
        private readonly StringBuilder sb = new StringBuilder();
        private readonly ILogger<GachaHandler> logger;
        private readonly GachaImageRender render;

        public GachaHandler(GarupaData data, BestdoriClient client, ILogger<GachaHandler> logger, GachaImageRender render)
        {
            this.data = data;
            this.client = client;
            stars = new Dictionary<int, string>()
            {
                { 1, "★1" },
                { 2, "★2" },
                { 3, "★3" },
                { 4, "★4" }
            };
            this.logger = logger;
            this.render = render;
        }
        public async void DoHandleAsync(MiraiHttpSession session, IMessageBase[] chain, IBaseInfo info, bool isGroupMessage = true)
        {
            List<IMessageBase> result = null;
            IMessageBase img = null;
            sb.Clear();
            switch (chain.First(m => m.Type == "Plain").ToString())
            {
                case "单抽":
                    logger.LogInformation($"{info.Name} 单抽");
                    var card = await GachaSingle();
                    var message = $"单抽结果: {stars[card.Item2.Rarity]} {data.Characters[card.Item2.CharacterId.ToString()].CharacterName[0]} - {card.Item2.Prefix[0]}";
                    img = await session.UploadPictureAsync(UploadTarget.Group, await client.GetCardThumbPath(card.Item2.ResourceSetName, card.Item1));
                    result = new List<IMessageBase>
                    {
                        new PlainMessage(message),
                        img
                    };
                    logger.LogInformation(message);
                    break;
                case "当期池子":
                    GachaDetail gd = GetRecentGachaDetail();
                    DateTime endTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(gd.ClosedAt[0])).LocalDateTime;
                    DateTime startTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(gd.PublishedAt[0])).LocalDateTime;
                    sb.AppendLine(gd.GachaName[0]);
                    sb.AppendLine(gd.Information.NewMemberInfo[0]);
                    sb.AppendLine($"{startTime} - {endTime}");
                    img = await session.UploadPictureAsync(UploadTarget.Group, await client.GetGachaBannerImagePath(gd.BannerAssetBundleName));
                    result = new List<IMessageBase>
                    {
                        new PlainMessage(sb.ToString()),
                        img
                    };
                    logger.LogInformation(sb.ToString());
                    break;
                case "十连":
                    List<(string, Card)> cards = new List<(string, Card)>(11);
                    for (int i = 0; i < 9; i++)
                    {
                        cards.Add(await GachaSingle());
                    }
                    cards.Add(await GachaSingle(tenTimes: true));
                    img = await session.UploadPictureAsync(UploadTarget.Group, await render.RenderGachaImageAsync(cards));
                    sb.AppendLine("十连结果:");
                    foreach (var item in cards)
                    {
                        sb.AppendLine($"{stars[item.Item2.Rarity]} {data.Characters[item.Item2.CharacterId.ToString()].CharacterName[0]} - {item.Item2.Prefix[0]}");
                    }
                    result = new List<IMessageBase>
                    {
                        new PlainMessage(sb.ToString()),
                        img
                    };
                    logger.LogInformation(sb.ToString());
                    break;
                default:
                    break;
            }
            if (isGroupMessage)
            {
                await session.SendGroupMessageAsync(((IGroupMemberInfo)info).Group.Id, result.ToArray());
            }
            else
            {
                await session.SendFriendMessageAsync(info.Id, result.ToArray());
            }
        }

        private async Task<(string, Card)> GachaSingle(string id = null, bool tenTimes = false)
        {
            GachaDetail gd = !string.IsNullOrWhiteSpace(id) ? await client.GetGacha(id) : data.RecentGachaDetails.FirstOrDefault(i => i.Value.Type == "permanent" || i.Value.Type == "limited").Value;
            if (gd == null)
            {
                gd = data.RecentGachaDetails.Last().Value;
            }
            var cards = gd.Details[0].ToList();
            var rates = gd.Rates[0].ToList();
            // 1. 选择稀有度
            double sumRate = rates.Sum(c => c.Value.Rate);
            double tempRate = 0;
            List<double> sorted = new List<double>();
            rates.ForEach(r => { tempRate += r.Value.Rate; sorted.Add(tempRate / sumRate); });
            double next = randomer.NextDouble();
            sorted.Add(next);
            sorted.Sort();
            var rate = int.Parse(rates[sorted.IndexOf(next)].Key);
            // 2. 十连三星保底
            if (tenTimes == true && rate == 2)
            {
                rate = 3;
            }
            // 3. 取出相应卡组
            cards = cards.Where(c => c.Value.RarityIndex == rate).ToList();
            sorted.Clear();
            tempRate = 0;
            sumRate = 0;
            // 4. 大于三星时选择概率提升卡组
            if (rate > 3)
            {
                var pickups = cards.GroupBy(k => k.Value.Pickup).Select(g => new KeyValuePair<bool, double>(g.Key, g.Sum(k => k.Value.Weight))).ToList();
                sumRate = pickups.Sum(k => k.Value);
                pickups.ForEach(p => { tempRate += p.Value; sorted.Add(tempRate / sumRate); });
                next = randomer.NextDouble();
                sorted.Add(next);
                sorted.Sort();
                var pickup = pickups[sorted.IndexOf(next)].Key;
                cards = cards.Where(k => k.Value.Pickup == pickup).ToList();
                sorted.Clear();
                tempRate = 0;
                sumRate = 0;
            }
            // 5. 取出卡片
            int nextIndex = randomer.Next(0, cards.Count - 1);
            return (cards[nextIndex].Key, data.Cards[cards[nextIndex].Key]);
        }

        private GachaDetail GetRecentGachaDetail()
        {
            return data.RecentGachaDetails.FirstOrDefault(i => i.Value.Type == "permanent" || i.Value.Type == "limited").Value;
        }
    }
}
