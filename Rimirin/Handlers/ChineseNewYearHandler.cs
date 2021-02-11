using Mirai_CSharp;
using Mirai_CSharp.Models;
using Rimirin.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Handlers
{
    [HandlerKey("^新年快乐$")]
    [HandlerKey("^新年抽签$")]
    public class ChineseNewYearHandler : IHandler, IGroupMessageHandler
    {
        private readonly Random randomer = new Random();
        public async void DoHandleAsync(MiraiHttpSession session, IMessageBase[] chain, IBaseInfo info, bool isGroupMessage = true)
        {
            ChineseLunisolarCalendar calendar = new ChineseLunisolarCalendar();
            var now = DateTime.Now;
            var year = calendar.GetYear(now);
            var month = calendar.GetMonth(now);
            var day = calendar.GetDayOfMonth(now);
            var hasLeapMonth = calendar.GetLeapMonth(year) > 0;
            if (hasLeapMonth)
            {
                if (month < 13 && day < 23)
                {
                    if (month > 1 && day > 15)
                    {
                        return;
                    }
                }
            }
            else
            {
                if (month < 12 && day < 23)
                {
                    if (month > 1 && day > 15)
                    {
                        return;
                    }
                }
            }
            List<IMessageBase> result = new List<IMessageBase>();
            IMessageBase img = null;
            switch (chain.First(m => m.Type == "Plain").ToString())
            {
                case "新年快乐":
                    img = await session.UploadPictureAsync(UploadTarget.Group, "Resources\\NewYear\\NewYearStick.png");
                    result.Add(img);
                    break;
                case "新年抽签":
                    string[] lotterise = Directory.GetFiles("Resources\\Lottery");
                    img = await session.UploadPictureAsync(UploadTarget.Group, lotterise[randomer.Next(0, lotterise.Length - 1)]);
                    result.Add(img);
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
    }
}
