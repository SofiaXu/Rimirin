using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin;
using Mirai_CSharp.Plugin.Interfaces;
using Rimirin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Plugins
{
    public class FriendPlugin : IPlugin, IFriendMessage
    {
        private readonly Dictionary<string, IFriendMessageHandler> handlers = new Dictionary<string, IFriendMessageHandler>();
        public FriendPlugin(IServiceProvider serviceProvider)
        {
            var plugins = Assembly.GetExecutingAssembly().DefinedTypes.Where(ti => ti.GetInterface("IFriendMessageHandler") != null).ToArray();
            foreach (var plugin in plugins)
            {
                foreach (HandlerKeyAttribute attribute in Attribute.GetCustomAttributes(plugin, typeof(HandlerKeyAttribute)))
                {
                    handlers.Add(attribute.Keyword, (IFriendMessageHandler)serviceProvider.GetService(plugin.AsType()));
                }
            }
        }
        public Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e)
        {
            var text = e.Chain.FirstOrDefault(m => m.Type == "Plain").ToString();
            handlers.Where(h => h.Key == text).ToList().ForEach(h => h.Value.DoHandle(session, e.Chain, e.Sender));
            return new Task<bool>(() => false);
        }
    }
}
