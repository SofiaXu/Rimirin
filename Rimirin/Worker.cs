using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mirai_CSharp;
using Mirai_CSharp.Plugin;
using Rimirin.Garupa;
using Rimirin.Models.Garupa;
using Rimirin.Framework.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rimirin
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MiraiHttpSession _session;
        private readonly IOptions<SessionOptions> _SessionOptions;
        private readonly BestdoriClient _client;
        private readonly GarupaData _data;
        private readonly IServiceProvider _serviceProvider;
        private readonly System.Timers.Timer _timer;

        public Worker(ILogger<Worker> logger, MiraiHttpSession session,
            IOptions<SessionOptions> SessionOptions, GarupaData data,
            IServiceProvider serviceProvider, BestdoriClient client)
        {
            _logger = logger;
            _session = session;
            _SessionOptions = SessionOptions;
            _client = client;
            _data = data;
            _serviceProvider = serviceProvider;
            _timer = new System.Timers.Timer(TimeSpan.FromMinutes(60).TotalMilliseconds);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Run();
            }
            finally
            {
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// 启动
        /// </summary>
        private void Run()
        {
            _logger.LogInformation("Rimirin Bot启动中...");
#if DEBUG
            var updateFromNetwork = false;
#else
            var updateFromNetwork = true;
#endif
            UpdateFiles(updateFromNetwork);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
#if DEBUG
            //OnTimerElapsed(null, null);
#endif
        }

        /// <summary>
        /// 定时更新文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateFiles();
        }

        private async void UpdateFiles(bool updateFromNetwork = true)
        {
            _logger.LogInformation("正在更新活动列表");
            _data.Events = await _client.GetEvents(updateFromNetwork);
            _logger.LogInformation("活动列表更新完成");
            _logger.LogInformation("正在更新卡片列表");
            _data.Cards = await _client.GetCards(updateFromNetwork);
            _logger.LogInformation("卡片列表更新完成");
            _logger.LogInformation("正在更新卡池列表");
            _data.Gacha = await _client.GetGacha(updateFromNetwork);
            _logger.LogInformation("卡池列表更新完成");
            _logger.LogInformation("正在更新角色列表");
            _data.Characters = await _client.GetCharacters(updateFromNetwork);
            _logger.LogInformation("角色列表更新完成");
            _logger.LogInformation("正在更新乐队列表");
            _data.Bands = await _client.GetBands(updateFromNetwork);
            _logger.LogInformation("乐队列表更新完成");
            _logger.LogInformation("正在更新当期卡池详情");
            var recentList = _data.GetRecentGacha();
            var recentDetail = new Dictionary<string, GachaDetail>();
            foreach (var item in recentList)
            {
                recentDetail.Add(item.Key, await _client.GetGacha(item.Key, updateFromNetwork));
            }
            _data.RecentGachaDetails = recentDetail;
            _logger.LogInformation("当期卡池详情更新完成");
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