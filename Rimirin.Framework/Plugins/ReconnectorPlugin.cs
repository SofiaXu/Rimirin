using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin;
using Mirai_CSharp.Plugin.Interfaces;
using Rimirin.Framework.Options;
using System;
using System.Threading.Tasks;

namespace Rimirin.Framework.Plugins
{
    /// <summary>
    /// 自动重新连接
    /// </summary>
    public class ReconnectorPlugin : IPlugin, IDisconnected
    {
        private readonly IOptions<SessionOptions> miraiSessionOptions;
        private readonly ILogger<ReconnectorPlugin> logger;

        public ReconnectorPlugin(IOptions<SessionOptions> miraiSessionOptions, ILogger<ReconnectorPlugin> logger)
        {
            this.miraiSessionOptions = miraiSessionOptions;
            this.logger = logger;
        }

        public async Task<bool> Disconnected(MiraiHttpSession session, IDisconnectedEventArgs e)
        {
            logger.LogError("已断开连接，正在尝试重连");
            int counter = 0;
            MiraiHttpSessionOptions options = new MiraiHttpSessionOptions(miraiSessionOptions.Value.MiraiHost, miraiSessionOptions.Value.MiraiHostPort, miraiSessionOptions.Value.MiraiSessionKey);
            while (true)
            {
                try
                {
                    logger.LogError($"正在尝试重连，第{counter}次");
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