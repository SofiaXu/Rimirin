using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mirai_CSharp;
using Mirai_CSharp.Plugin;
using Rimirin.Garupa;
using Rimirin.Models.Garupa;
using Rimirin.Options;
using Rimirin.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rimirin
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MiraiHttpSession _session;
        private readonly IOptions<MiraiSessionOptions> _miraiSessionOptions;
        private readonly BestdoriClient _client;
        private readonly GarupaData _data;
        private readonly IServiceProvider _serviceProvider;
        private readonly System.Timers.Timer _timer;

        public Worker(ILogger<Worker> logger, MiraiHttpSession session,
            IOptions<MiraiSessionOptions> miraiSessionOptions, GarupaData data,
            IServiceProvider serviceProvider, BestdoriClient client)
        {
            _logger = logger;
            _session = session;
            _miraiSessionOptions = miraiSessionOptions;
            _client = client;
            _data = data;
            _serviceProvider = serviceProvider;
            _timer = new System.Timers.Timer(TimeSpan.FromHours(1).TotalMilliseconds);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Run();
                while (!stoppingToken.IsCancellationRequested) ;
            }
            finally
            {
                Stop();
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// 启动
        /// </summary>
        private async void Run()
        {
            _logger.LogInformation("Rimirin Bot启动中...");
#if DEBUG
            var updateFromNetwork = false;
#else
            var updateFromNetwork = true;
#endif
            _logger.LogInformation("正在更新活动列表");
            _data.Events = await _client.GetEvents(updateFromNetwork);
            _logger.LogInformation("活动列表更新完成");
            var plugins = Assembly.GetExecutingAssembly().DefinedTypes.Where(ti => ti.GetInterface("IPlugin") != null).ToArray();
            foreach (var plugin in plugins)
            {
                _session.AddPlugin((IPlugin)_serviceProvider.GetService(plugin.AsType()));
            }
            _timer.Elapsed += UpdateFiles;
            _timer.Start();
            await _session.ConnectAsync(new Mirai_CSharp.Models.MiraiHttpSessionOptions(_miraiSessionOptions.Value.MiraiHost, _miraiSessionOptions.Value.MiraiHostPort, _miraiSessionOptions.Value.MiraiSessionKey), _miraiSessionOptions.Value.MiraiSessionQQ);
        }

        /// <summary>
        /// 定时更新文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateFiles(object sender, System.Timers.ElapsedEventArgs e)
        {

        }

        private void Stop()
        {
            Dispose();
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _session.DisposeAsync().AsTask().Wait();
                    _client.Dispose();
                    _timer.Dispose();
                }

                disposedValue = true;
            }
        }

        public override void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
