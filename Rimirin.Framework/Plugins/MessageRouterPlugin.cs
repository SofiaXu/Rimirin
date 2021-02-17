using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin;
using Mirai_CSharp.Plugin.Interfaces;
using Rimirin.Framework.Handlers.Announces;
using Rimirin.Framework.Handlers.Interfaces;
using Rimirin.Framework.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rimirin.Framework.Plugins
{
    /// <summary>
    /// 消息路由器用于路由好友消息（<see cref="IFriendMessageHandler"/>）、群消息（<see cref="IGroupMessageHandler"/>）和临时消息（<see cref="ITempMessageHandler"/>）
    /// </summary>
    public class MessageRouterPlugin : IPlugin, IGroupMessage, IFriendMessage, ITempMessage
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<MessageRouterPlugin> logger;
        private readonly Dictionary<MessageHandlerAttribute, TypeInfo> friendHandlers = new Dictionary<MessageHandlerAttribute, TypeInfo>();
        private readonly Dictionary<MessageHandlerAttribute, TypeInfo> groupHandlers = new Dictionary<MessageHandlerAttribute, TypeInfo>();
        private readonly Dictionary<MessageHandlerAttribute, TypeInfo> tempHandlers = new Dictionary<MessageHandlerAttribute, TypeInfo>();

        public MessageRouterPlugin(IServiceProvider serviceProvider, ILogger<MessageRouterPlugin> logger, IOptions<HandlerOptions> options)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            var plugins = options.Value.Handlers.Where(ti => ti.GetInterface(nameof(IGroupMessageHandler)) != null && ti.IsClass == true && ti.IsAbstract == false).ToArray();
            foreach (var plugin in plugins)
            {
                foreach (MessageHandlerAttribute attribute in Attribute.GetCustomAttributes(plugin, typeof(MessageHandlerAttribute)))
                {
                    groupHandlers.Add(attribute, plugin);
                }
            }
            plugins = options.Value.Handlers.Where(ti => ti.GetInterface(nameof(IFriendMessageHandler)) != null && ti.IsClass == true && ti.IsAbstract == false).ToArray();
            foreach (var plugin in plugins)
            {
                foreach (MessageHandlerAttribute attribute in Attribute.GetCustomAttributes(plugin, typeof(MessageHandlerAttribute)))
                {
                    friendHandlers.Add(attribute, plugin);
                }
            }
            plugins = options.Value.Handlers.Where(ti => ti.GetInterface(nameof(ITempMessageHandler)) != null && ti.IsClass == true && ti.IsAbstract == false).ToArray();
            foreach (var plugin in plugins)
            {
                foreach (MessageHandlerAttribute attribute in Attribute.GetCustomAttributes(plugin, typeof(MessageHandlerAttribute)))
                {
                    tempHandlers.Add(attribute, plugin);
                }
            }
        }

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e)
        {
            var text = e.Chain.FirstOrDefault(m => m.Type == "Plain")?.ToString();
            if (text == null)
            {
                return false;
            }
            var handler = friendHandlers.Where(h => Regex.IsMatch(text, h.Key.Regex)).FirstOrDefault();
            if (handler.Value == null)
            {
                return false;
            }
            logger.LogInformation($"来自好友\"{e.Sender.Name}\"{{{e.Sender.Id}}}匹配\"{handler.Key.Regex}\"的好友消息：{string.Join(null, e.Chain.AsEnumerable())}");
            logger.LogInformation($"已找到好友消息处理器：{handler.Key.HandlerName ?? handler.Value.FullName}");
            using (var sp = serviceProvider.CreateScope())
            {
                try
                {
                    await ((IFriendMessageHandler)sp.ServiceProvider.GetRequiredService(handler.Value.AsType()))?.DoHandle(session, e.Chain, e.Sender);
                }
                catch (Exception ex)
                {
                    logger.LogError($"消息处理器{(handler.Key.HandlerName is null ? handler.Value.FullName : handler.Key.HandlerName)}发生未经处理的异常：\n{ex.Message}\n堆栈追踪：\n{ex.StackTrace}");
                }
            }
            return false;
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e)
        {
            var text = e.Chain.FirstOrDefault(m => m.Type == "Plain")?.ToString();
            if (text == null)
            {
                return false;
            }
            var handler = groupHandlers.Where(h => Regex.IsMatch(text, h.Key.Regex)).FirstOrDefault();
            if (handler.Value == null)
            {
                return false;
            }
            logger.LogInformation($"来自群\"{e.Sender.Group.Name}\"{{{e.Sender.Group.Id}}}成员\"{e.Sender.Name}\"{{{e.Sender.Id}}}匹配\"{handler.Key.Regex}\"的群消息：{string.Join(null, e.Chain.AsEnumerable())}");
            logger.LogInformation($"已找到群消息处理器：{handler.Key.HandlerName ?? handler.Value.FullName}");
            using (var sp = serviceProvider.CreateScope())
            {
                try
                {
                    await ((IGroupMessageHandler)sp.ServiceProvider.GetRequiredService(handler.Value.AsType())).DoHandle(session, e.Chain, e.Sender);
                }
                catch (Exception ex)
                {
                    logger.LogError($"消息处理器{handler.Key.HandlerName ?? handler.Value.FullName}发生未经处理的异常：\n{ex.Message}\n堆栈追踪：\n{ex.StackTrace}");
                }
            }
            return false;
        }

        public async Task<bool> TempMessage(MiraiHttpSession session, ITempMessageEventArgs e)
        {
            var text = e.Chain.FirstOrDefault(m => m.Type == "Plain")?.ToString();
            if (text == null)
            {
                return false;
            }
            var handler = tempHandlers.Where(h => Regex.IsMatch(text, h.Key.Regex)).FirstOrDefault();
            if (handler.Value == null)
            {
                return false;
            }
            logger.LogInformation($"来自群\"{e.Sender.Group.Name}\"{{{e.Sender.Group.Id}}}成员\"{e.Sender.Name}\"{{{e.Sender.Id}}}匹配\"{handler.Key.Regex}\"的临时消息：{string.Join(null, e.Chain.AsEnumerable())}");
            logger.LogInformation($"已找到临时消息处理器：{handler.Key.HandlerName ?? handler.Value.FullName}");
            using (var sp = serviceProvider.CreateScope())
            {
                try
                {
                    await ((ITempMessageHandler)sp.ServiceProvider.GetRequiredService(handler.Value.AsType())).DoHandle(session, e.Chain, e.Sender);
                }
                catch (Exception ex)
                {
                    logger.LogError($"消息处理器{handler.Key.HandlerName ?? handler.Value.FullName}发生未经处理的异常：\n{ex.Message}\n堆栈追踪：\n{ex.StackTrace}");
                }
            }
            return false;
        }
    }
}