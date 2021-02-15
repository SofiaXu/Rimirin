using Microsoft.Extensions.Options;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Rimirin.Framework.Handlers.Announces;
using Rimirin.Framework.Handlers.Interfaces;
using Rimirin.Framework.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Framework.Handlers
{
    [MessageHandler("^帮助$", "Rimirin框架内置帮助显示", "帮助", "显示帮助文档")]
    public class HelpHandler : IMessageHandler, IGroupMessageHandler, IFriendMessageHandler, ITempMessageHandler
    {
        private readonly IOptions<HandlerOptions> options;

        public HelpHandler(IOptions<HandlerOptions> options)
        {
            this.options = options;
        }

        async Task ITempMessageHandler.DoHandle(MiraiHttpSession session, IMessageBase[] chain, IGroupMemberInfo info)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.AppendLine("帮助");
            Dictionary<MessageHandlerAttribute, TypeInfo> handlers = new Dictionary<MessageHandlerAttribute, TypeInfo>();
            var plugins = options.Value.Handlers.Where(ti => ti.GetInterface("ITempMessageHandler") != null).ToArray();
            foreach (var plugin in plugins)
            {
                foreach (MessageHandlerAttribute attribute in Attribute.GetCustomAttributes(plugin, typeof(MessageHandlerAttribute)))
                {
                    handlers.Add(attribute, plugin);
                }
            }
            handlers.GroupBy(k => k.Key.HandlerName ?? k.Value.FullName, v => v.Key).ToList().ForEach(g =>
            {
                sb.AppendLine($"模块名：{g.Key}");
                foreach (var item in g)
                {
                    sb.AppendLine($"命令：{item.HelpCommand ?? item.Regex}");
                    if (item.HelpText != null) sb.AppendLine($"帮助：{item.HelpText}");
                }
                sb.AppendLine();
            });
            var result = new IMessageBase[]
            {
                new PlainMessage(sb.ToString())
            };
            await session.SendTempMessageAsync(info.Id, info.Group.Id, result);
        }

        async Task IGroupMessageHandler.DoHandle(MiraiHttpSession session, IMessageBase[] chain, IGroupMemberInfo info)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.AppendLine("帮助");
            Dictionary<MessageHandlerAttribute, TypeInfo> handlers = new Dictionary<MessageHandlerAttribute, TypeInfo>();
            var plugins = options.Value.Handlers.Where(ti => ti.GetInterface("IGroupMessageHandler") != null).ToArray();
            foreach (var plugin in plugins)
            {
                foreach (MessageHandlerAttribute attribute in Attribute.GetCustomAttributes(plugin, typeof(MessageHandlerAttribute)))
                {
                    handlers.Add(attribute, plugin);
                }
            }
            handlers.GroupBy(k => k.Key.HandlerName ?? k.Value.FullName, v => v.Key).ToList().ForEach(g =>
            {
                sb.AppendLine($"模块名：{g.Key}");
                foreach (var item in g)
                {
                    sb.AppendLine($"命令：{item.HelpCommand ?? item.Regex}");
                    if (item.HelpText != null) sb.AppendLine($"帮助：{item.HelpText}");
                }
                sb.AppendLine();
            });
            var result = new IMessageBase[]
            {
                new PlainMessage(sb.ToString())
            };
            await session.SendGroupMessageAsync(info.Group.Id, result);
        }

        public async Task DoHandle(MiraiHttpSession session, IMessageBase[] chain, IFriendInfo info)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.AppendLine("帮助");
            Dictionary<MessageHandlerAttribute, TypeInfo> handlers = new Dictionary<MessageHandlerAttribute, TypeInfo>();
            var plugins = options.Value.Handlers.Where(ti => ti.GetInterface("IFriendMessageHandler") != null).ToArray();
            foreach (var plugin in plugins)
            {
                foreach (MessageHandlerAttribute attribute in Attribute.GetCustomAttributes(plugin, typeof(MessageHandlerAttribute)))
                {
                    handlers.Add(attribute, plugin);
                }
            }
            handlers.GroupBy(k => k.Key.HandlerName ?? k.Value.FullName, v => v.Key).ToList().ForEach(g =>
            {
                sb.AppendLine($"模块名：{g.Key}");
                foreach (var item in g)
                {
                    sb.AppendLine($"命令：{item.HelpCommand ?? item.Regex}");
                    if (item.HelpText != null) sb.AppendLine($"帮助：{item.HelpText}");
                }
                sb.AppendLine();
            });
            var result = new IMessageBase[]
            {
                new PlainMessage(sb.ToString())
            };
            await session.SendFriendMessageAsync(info.Id, result);
        }
    }
}