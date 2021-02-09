using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin;
using Mirai_CSharp.Plugin.Interfaces;
using Rimirin.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimirin.Plugins
{
    public class DisconnectedPlugin : IPlugin, IDisconnected
    {
        private readonly IOptions<MiraiSessionOptions> miraiSessionOptions;
        private readonly ILogger<DisconnectedPlugin> logger;
        public DisconnectedPlugin(IOptions<MiraiSessionOptions> miraiSessionOptions, ILogger<DisconnectedPlugin> logger)
        {
            this.miraiSessionOptions = miraiSessionOptions;
            this.logger = logger;
        }
        public async Task<bool> Disconnected(MiraiHttpSession session, IDisconnectedEventArgs e)
        {
            logger.LogError("已断开连接，正在尝试重连");
            MiraiHttpSessionOptions options = new MiraiHttpSessionOptions(miraiSessionOptions.Value.MiraiHost, miraiSessionOptions.Value.MiraiHostPort, miraiSessionOptions.Value.MiraiSessionKey);
            while (true)
            {
                try
                {
                    await session.ConnectAsync(options, miraiSessionOptions.Value.MiraiSessionQQ);
                    logger.LogInformation("重连成功");
                    return true;
                }
                catch (Exception)
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}
