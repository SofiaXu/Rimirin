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
        /// ����
        /// </summary>
        private void Run()
        {
            _logger.LogInformation("Rimirin Bot������...");
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
        /// ��ʱ�����ļ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateFiles();
        }

        private async void UpdateFiles(bool updateFromNetwork = true)
        {
            _logger.LogInformation("���ڸ��»�б�");
            _data.Events = await _client.GetEvents(updateFromNetwork);
            _logger.LogInformation("��б�������");
            _logger.LogInformation("���ڸ��¿�Ƭ�б�");
            _data.Cards = await _client.GetCards(updateFromNetwork);
            _logger.LogInformation("��Ƭ�б�������");
            _logger.LogInformation("���ڸ��¿����б�");
            _data.Gacha = await _client.GetGacha(updateFromNetwork);
            _logger.LogInformation("�����б�������");
            _logger.LogInformation("���ڸ��½�ɫ�б�");
            _data.Characters = await _client.GetCharacters(updateFromNetwork);
            _logger.LogInformation("��ɫ�б�������");
            _logger.LogInformation("���ڸ����ֶ��б�");
            _data.Bands = await _client.GetBands(updateFromNetwork);
            _logger.LogInformation("�ֶ��б�������");
            _logger.LogInformation("���ڸ��µ��ڿ�������");
            var recentList = _data.GetRecentGacha();
            var recentDetail = new Dictionary<string, GachaDetail>();
            foreach (var item in recentList)
            {
                recentDetail.Add(item.Key, await _client.GetGacha(item.Key, updateFromNetwork));
            }
            _data.RecentGachaDetails = recentDetail;
            _logger.LogInformation("���ڿ�������������");
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