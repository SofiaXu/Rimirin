using Microsoft.Extensions.Logging;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Rimirin.Framework.Handlers.Announces;
using Rimirin.Framework.Handlers.Interfaces;
using Rimirin.Garupa;
using Rimirin.Models.Garupa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rimirin.Handlers
{
    [MessageHandler("^十连三星必得$", "邦邦抽卡", "十连三星必得", "十连抽当前三星必得卡池")]
    [MessageHandler("^十连(?: (\\d+))?$", "邦邦抽卡", "十连 {卡池编号}",
        "十连抽指定编号卡池，可在bestdori查询编号，为空则抽当前活动卡池，仅限单抽的池子不可抽取")]
    [MessageHandler("^单抽(?: (\\d+))?$", "邦邦抽卡", "单抽 {卡池编号}", "单抽指定编号卡池，可在bestdori查询编号，为空则抽当前活动卡池")]
    [MessageHandler("^(当前)?活动卡池$", "邦邦抽卡", "当前活动卡池", "显示当前活动卡池的信息")]
    [MessageHandler("^当前卡池|未结束卡池$", "邦邦抽卡", "当前卡池、未结束卡池", "显示未结束卡池的信息")]
    [MessageHandler("^未来卡池|未开始卡池$", "邦邦抽卡", "未来卡池、未开始卡池", "显示未开始卡池的信息")]
    //[MessageHandler(@"^查询卡池 (?:(?:卡池编号 (?<GachaNumber>\d+))|(?:卡池名称 (?<GachaName>.*))|(?:卡池开始时间 (?<GachaStart>\d{4}年\d{1,2}月\d{1,2}日)))$",
    //    "邦邦抽卡", "查询卡池 卡池名称 {卡池日语名称}、查询卡池 卡池名称 {卡池编号}、查询卡池 卡池开始时间 {xxxx年xx月xx日}", "查询指定卡池的信息")]
    public class GachaHandler : IHandler, IMessageHandler, IGroupMessageHandler
    {
        private readonly string[] patterns;
        private readonly GarupaData data;
        private readonly BestdoriClient client;
        private readonly Random randomer = new Random();
        private readonly Dictionary<int, string> stars;
        private readonly Dictionary<string, string> paymentName;
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
            patterns = new string[]
            {
                "^十连三星必得$",
                "^十连(?: (\\d+))?$",
                "^单抽(?: (\\d+))?$",
                "^(当前)?活动卡池$",
                "^当前卡池|未结束卡池$",
                "^未来卡池|未开始卡池$",
                //@"^查询卡池 (?:(?:卡池编号 (?<GachaNumber>\d+))|(?:卡池名称 (?<GachaName>.*))|(?:卡池开始时间 (?<GachaStart>\d{4}年\d{1,2}月\d{1,2}日)))$"
            };
            paymentName = new Dictionary<string, string>()
            {
                { "free_star", "星石（免费）" },
                { "paid_star", "星石（付费）" },
                { "over_the_3_star_ticket", "三星兑换券" },
                { "normal_ticket", "单次兑换券" },
                { "free", "免费" }
            };
            this.logger = logger;
            this.render = render;
        }
        public async Task DoHandle(MiraiHttpSession session, IMessageBase[] chain, IGroupMemberInfo info)
        {
            List<IMessageBase> result = new List<IMessageBase>();
            IMessageBase img = null;
            sb.Clear();
            var text = chain.First(m => m.Type == "Plain").ToString();
            var match = patterns.Select((p, i) => (i, Regex.Match(text, p))).First(i => i.Item2.Success);
            switch (match.i)
            {
                case 0:
                    {
                        var gd = data.RecentGachaDetails.LastOrDefault(i => i.Value.GachaName[0] == "★３以上確定チケットガチャ");
                        List<(string id, Card)> cards = Gacha(gd.Value, gd.Key, true);
                        result.Add(new PlainMessage(sb.ToString()));
                        if (cards != null)
                        {
                            using var s = await render.RenderGachaImageAsync(cards);
                            img = await session.UploadPictureAsync(UploadTarget.Group, s);
                            result.Add(img);
                        }
                        break;
                    }
                case 1:
                case 2:
                    {
                        List<(string id, Card)> cards = null;
                        if (match.Item2.Groups[1].Success)
                        {
                            var id = match.Item2.Groups[1].Value;
                            if (!data.Gacha.TryGetValue(id, out _))
                            {
                                result.Add(new PlainMessage($"未查询到卡池{id}"));
                                break;
                            }
                            cards = Gacha(await client.GetGacha(id), id, match.i == 1);
                        }
                        else
                        {
                            cards = Gacha(tenTimes: match.i == 1);
                        }
                        result.Add(new PlainMessage(sb.ToString()));
                        if (cards != null)
                        {
                            using var s = await render.RenderGachaImageAsync(cards);
                            img = await session.UploadPictureAsync(UploadTarget.Group, s);
                            result.Add(img);
                        }
                        break;
                    }
                case 3:
                    {
                        var gd = GetRecentGachaDetail();
                        DateTime endTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(gd.Value.ClosedAt[0])).LocalDateTime;
                        DateTime startTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(gd.Value.PublishedAt[0])).LocalDateTime;
                        sb.AppendLine($"卡池编号：{gd.Key}");
                        sb.AppendLine($"卡池名称：{gd.Value.GachaName[0]}");
                        if (gd.Value.Information.NewMemberInfo[0] != null)
                            sb.AppendLine($"新角色介绍：{gd.Value.Information.NewMemberInfo[0]}");
                        sb.AppendLine($"抽取方式：");
                        foreach (var payment in gd.Value.PaymentMethods)
                        {
                            if (!this.paymentName.TryGetValue(payment.PaymentMethod, out string paymentName))
                            {
                                paymentName = payment.PaymentMethod;
                            }
                            sb.AppendLine($"{(payment.CostItemQuantity > 0 ? payment.CostItemQuantity : "")}{paymentName}");
                        }
                        sb.AppendLine($"可抽时间：{startTime} - {endTime}");
                        result.Add(new PlainMessage(sb.ToString()));
                        if (gd.Value.BannerAssetBundleName != null)
                        {
                            img = await session.UploadPictureAsync(UploadTarget.Group, await client.GetGachaBannerImagePath(gd.Value.BannerAssetBundleName));
                            result.Add(img);
                        }
                        break;
                    }
                case 4:
                    {
                        string log = "";
                        foreach (var gd in data.RecentGachaDetails)
                        {
                            DateTime endTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(gd.Value.ClosedAt[0])).LocalDateTime;
                            DateTime startTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(gd.Value.PublishedAt[0])).LocalDateTime;
                            if (endTime.Year > (DateTime.Now.Year + 2))
                            {
                                continue;
                            }
                            sb.AppendLine($"卡池编号：{gd.Key}");
                            sb.AppendLine($"卡池名称：{gd.Value.GachaName[0]}");
                            if (gd.Value.Information.NewMemberInfo[0] != null)
                                sb.AppendLine($"新角色介绍：{gd.Value.Information.NewMemberInfo[0]}");
                            sb.AppendLine($"抽取方式：");
                            foreach (var payment in gd.Value.PaymentMethods)
                            {
                                if (!this.paymentName.TryGetValue(payment.PaymentMethod, out string paymentName))
                                {
                                    paymentName = payment.PaymentMethod;
                                }
                                sb.AppendLine($"{(payment.CostItemQuantity > 0 ? payment.CostItemQuantity : "")}{paymentName}");
                            }
                            sb.AppendLine($"可抽时间：{startTime} - {endTime}");
                            log += sb.ToString();
                            if (gd.Value.BannerAssetBundleName != null)
                            {
                                result.Add(new PlainMessage(sb.ToString()));
                                img = await session.UploadPictureAsync(UploadTarget.Group, await client.GetGachaBannerImagePath(gd.Value.BannerAssetBundleName));
                                result.Add(img);
                            }
                            else
                            {
                                sb.AppendLine("-----------");
                                result.Add(new PlainMessage(sb.ToString()));
                            }
                            sb.Clear();
                        }
                        sb.Append(log);
                        break;
                    }
                case 5:
                    {
                        string log = "";
                        var gs = data.Gacha.Where(k => !string.IsNullOrEmpty(k.Value.PublishedAt[0]) && DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(k.Value.PublishedAt[0])).LocalDateTime >= DateTime.Now).ToDictionary(k => k.Key, k => k.Value);
                        if (gs.Count == 0)
                        {
                            sb.Append("未发现未开始卡池");
                            result.Add(new PlainMessage(sb.ToString()));
                            break;
                        }
                        foreach (var g in gs)
                        {
                            var gd = await client.GetGacha(g.Key);
                            DateTime endTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(gd.ClosedAt[0])).LocalDateTime;
                            DateTime startTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(gd.PublishedAt[0])).LocalDateTime;
                            if (endTime.Year > (DateTime.Now.Year + 2))
                            {
                                continue;
                            }
                            sb.AppendLine($"卡池编号：{g.Key}");
                            sb.AppendLine($"卡池名称：{gd.GachaName[0]}");
                            if (gd.Information.NewMemberInfo[0] != null)
                                sb.AppendLine($"新角色介绍：{gd.Information.NewMemberInfo[0]}");
                            sb.AppendLine($"抽取方式：");
                            foreach (var payment in gd.PaymentMethods)
                            {
                                if (!this.paymentName.TryGetValue(payment.PaymentMethod, out string paymentName))
                                {
                                    paymentName = payment.PaymentMethod;
                                }
                                sb.AppendLine($"{(payment.CostItemQuantity > 0 ? payment.CostItemQuantity : "")}{paymentName}");
                            }
                            sb.AppendLine($"可抽时间：{startTime} - {endTime}");
                            log += sb.ToString();
                            if (gd.BannerAssetBundleName != null)
                            {
                                result.Add(new PlainMessage(sb.ToString()));
                                img = await session.UploadPictureAsync(UploadTarget.Group, await client.GetGachaBannerImagePath(gd.BannerAssetBundleName));
                                result.Add(img);
                            }
                            else
                            {
                                sb.AppendLine("-----------");
                                result.Add(new PlainMessage(sb.ToString()));
                            }
                            sb.Clear();
                        }
                        sb.Append(log);
                        break;
                    }
                default:
                    break;
            }
            logger.LogInformation(sb.ToString());
            await session.SendGroupMessageAsync(groupNumber: info.Group.Id, result.ToArray(), ((SourceMessage)chain.First()).Id);
        }

        /// <summary>
        /// 抽卡
        /// </summary>
        /// <param name="gd">卡池</param>
        /// <param name="id">卡池编号</param>
        /// <param name="tenTimes">是否十连</param>
        /// <returns>抽中卡组</returns>
        private List<(string id, Card)> Gacha(GachaDetail gd = null, string id = null, bool tenTimes = false)
        {
            if (gd == null)
            {
                (id, gd) = GetRecentGachaDetail();
            }
            List<(string id, Card)> result = new List<(string id, Card)>();
            var maxCount = 1;
            if (tenTimes)
            {
                maxCount = gd.PaymentMethods.Max(p => p.Count);
                if (maxCount == 1)
                {
                    sb.AppendLine($"卡池{id}不可复数次抽取");
                    return null;
                }
            }
            if (gd.PaymentMethods == null)
            {
                sb.AppendLine($"卡池{id}无可用抽取方式");
                return null;
            }
            var payment = gd.PaymentMethods.FirstOrDefault(i => i.Count == maxCount && (tenTimes || i.PaymentMethod != "paid_star"));
            if (tenTimes == false && gd.PaymentMethods.Length > 0 && payment == null)
            {
                if (!gd.PaymentMethods.Any(i => i.Count == maxCount))
                {
                    sb.AppendLine($"卡池{id}不可单次抽取");
                    return null;
                }                
                payment = gd.PaymentMethods.First(i => i.Count == maxCount);
            }
            if (payment == null)
            {
                sb.AppendLine($"卡池{id}卡牌不可抽取");
                return null;
            }
            if (GachaSingle(gd).Item1 == null)
            {
                sb.AppendLine($"卡池{id}卡牌不可抽取");
                return null;
            }
            // 抽取方式所造成的动作 fixed_4_star_once: 四星保底 normal: 正常2星 over_the_3_star_once: 三星保底 once_a_day: 正常2星 
            int tenTimesRate = payment.Behavior == "fixed_4_star_once" ? 4 : payment.Behavior == "over_the_3_star_once" ? 3 : 2;
            if (!this.paymentName.TryGetValue(payment.PaymentMethod, out string paymentName))
            {
                paymentName = payment.PaymentMethod;
            }
            sb.AppendLine($"卡池名称：{gd.GachaName[0]}");
            sb.AppendLine($"消费：{(payment.CostItemQuantity > 0 ? payment.CostItemQuantity : "")}{paymentName}");
            for (int i = 0; i < maxCount - 1; i++)
            {
                result.Add(GachaSingle(gd));
            }
            result.Add(GachaSingle(gd, tenTimesRate));
            if (maxCount == 1)
            {
                sb.AppendLine("单抽结果:");
            }
            else
            {
                sb.AppendLine($"{maxCount}连结果:");
            }
            foreach (var item in result)
            {
                sb.AppendLine($"{stars[item.Item2.Rarity]} {data.Characters[item.Item2.CharacterId.ToString()].CharacterName[0]} - {item.Item2.Prefix[0]}");
            }
            return result;
        }

        /// <summary>
        /// 单次抽取（使用加权随机算法）
        /// </summary>
        /// <param name="gd">卡池</param>
        /// <param name="tenTimesRate">保底星数</param>
        /// <returns>卡片</returns>
        private (string, Card) GachaSingle(GachaDetail gd, int tenTimesRate = 2)
        {
            if (gd.Details[0] == null || gd.Rates[0] == null)
            {
                return (null, null);
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
            // 2. 保底
            if (rate < tenTimesRate)
            {
                rate = tenTimesRate;
            }
            // 3. 取出相应卡组
            cards = cards.Where(c => c.Value.RarityIndex == rate).ToList();
            sorted.Clear();
            tempRate = 0;
            sumRate = 0;
            // 4. 大于三星时选择概率提升卡组
            if (rate > 3 && cards.Any(d => d.Value.Pickup))
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

        private KeyValuePair<string, GachaDetail> GetRecentGachaDetail()
        {
            return data.RecentGachaDetails.FirstOrDefault(i => i.Value.Type == "permanent" || i.Value.Type == "limited");
        }
    }
}
