using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin;
using Mirai_CSharp.Plugin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Plugins
{
    public class InvitedJoinGroupPlugin : IPlugin, IBotInvitedJoinGroup
    {
        public async Task<bool> BotInvitedJoinGroup(MiraiHttpSession session, IBotInvitedJoinGroupEventArgs e)
        {
            await session.HandleBotInvitedJoinGroupAsync(e, GroupApplyActions.Allow);
            return true;
        }
    }
}
