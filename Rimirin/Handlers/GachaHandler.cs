using Microsoft.Extensions.Logging;
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
    public class GachaHandler : IHandler, IMessageHandler, IGroupMessageHandler, IFriendMessageHandler
    {
        private readonly GarupaData data;
        private readonly BestdoriClient client;
        private readonly Random randomer = new Random();
        private readonly Dictionary<int, string> stars;
        private readonly StringBuilder sb = new StringBuilder();
        private readonly ILogger<GachaHandler> logger;

        public GachaHandler(GarupaData data, BestdoriClient client, ILogger<GachaHandler> logger)
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
        }
        public async void DoHandleAsync(MiraiHttpSession session, IMessageBase[] chain, IBaseInfo info, bool isGroupMessage = true)
        {
            IMessageBase[] result = null;
            sb.Clear();
            switch (chain.First(m => m.Type == "Plain").ToString())
            {
                case "单抽":
                    logger.LogInformation($"{info.Name} 单抽");
                    var card = await GachaSingle();
                    var message = $"单抽结果: {stars[card.Rarity]} {data.Characters[card.CharacterId.ToString()].CharacterName[0]} - {card.Prefix[0]}";
                    result = new IMessageBase[]
                    {
                        new PlainMessage(message)
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
                    var img = await session.UploadPictureAsync(UploadTarget.Friend, await client.GetGachaBannerImagePath(gd.BannerAssetBundleName));
                    result = new IMessageBase[]
                    {
                        new PlainMessage(sb.ToString()),
                        img
                    };
                    logger.LogInformation(sb.ToString());
                    break;
                case "十连":
                    List<Card> cards = new List<Card>();
                    for (int i = 0; i < 9; i++)
                    {
                        cards.Add(await GachaSingle());
                    }
                    cards.Add(await GachaSingle(tenTimes: true));
                    sb.AppendLine("十连结果:");
                    foreach (var item in cards)
                    {
                        sb.AppendLine($"{stars[item.Rarity]} {data.Characters[item.CharacterId.ToString()].CharacterName[0]} - {item.Prefix[0]}");
                    }
                    result = new IMessageBase[]
                    {
                        new PlainMessage(sb.ToString())
                    };
                    logger.LogInformation(sb.ToString());
                    break;
                default:
                    break;
            }
            if (isGroupMessage)
            {
                await session.SendGroupMessageAsync(info.Id, result);
            }
            else
            {
                await session.SendFriendMessageAsync(info.Id, result);
            }
        }

        private async Task<Card> GachaSingle(string id = null, bool tenTimes = false)
        {
            GachaDetail gd = !string.IsNullOrWhiteSpace(id) ? await client.GetGacha(id) : data.RecentGachaDetails.FirstOrDefault(i => i.Value.Type == "permanent" || i.Value.Type == "limited").Value;
            if (gd == null)
            {
                gd = data.RecentGachaDetails.Last().Value;
            }
            var cards = gd.Details[0].ToList();
            double sumRate = cards.Sum(c => c.Value.Weight);
            List<double> sorted = new List<double>();
            double tempRate = 0;
            if (tenTimes == true)
            {
                cards = cards.Where(k => k.Value.RarityIndex > 2).ToList();
            }
            cards = cards.OrderByDescending(k => k.Value.RarityIndex).ThenByDescending(k => k.Value.Weight).ToList();
            cards.ForEach(om => { tempRate += om.Value.Weight; sorted.Add(tempRate / sumRate); });
            double next = randomer.NextDouble();
            sorted.Add(next);
            sorted.Sort();
            return data.Cards[cards[sorted.IndexOf(next)].Key];
        }

        private GachaDetail GetRecentGachaDetail()
        {
            return data.RecentGachaDetails.FirstOrDefault(i => i.Value.Type == "permanent" || i.Value.Type == "limited").Value;
        }
    }
}
