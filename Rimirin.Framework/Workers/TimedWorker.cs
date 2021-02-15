using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mirai_CSharp;
using Rimirin.Framework.Handlers.Announces;
using Rimirin.Framework.Handlers.Interfaces;
using Rimirin.Framework.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rimirin.Framework.Workers
{
    public class TimedWorker : BackgroundService
    {
        private readonly ILogger<TimedWorker> _logger;
        private readonly MiraiHttpSession _session;
        private readonly System.Timers.Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<TimedHandlerItem> _handlers;

        public TimedWorker(ILogger<TimedWorker> logger, MiraiHttpSession session, IServiceProvider serviceProvider, IOptions<HandlerOptions> options)
        {
            _logger = logger;
            _session = session;
            _serviceProvider = serviceProvider;
            _timer = new System.Timers.Timer(TimeSpan.FromMinutes(10).TotalMilliseconds);
            _timer.Elapsed += Timer_Elapsed;
            _handlers = new List<TimedHandlerItem>();
            var plugins = options.Value.Handlers.Where(ti => ti.GetInterface(nameof(ITimedHandler)) != null && ti.IsClass == true && ti.IsAbstract == false).ToArray();
            foreach (var plugin in plugins)
            {
                foreach (TimedHandlerAttribute attribute in Attribute.GetCustomAttributes(plugin, typeof(TimedHandlerAttribute)))
                {
                    _handlers.Add(new TimedHandlerItem(DateTime.Now, attribute, plugin));
                }
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var handlers = _handlers.Where(h => h.LastAccessTime + h.HandlerAttribute.Interval >= e.SignalTime).ToList();
            if (handlers.Count == 0)
            {
                return;
            }
            using var sp = _serviceProvider.CreateScope();
            for (int i = 0; i < handlers.Count; i++)
            {
                _logger.LogInformation($"已触发定时处理器：{ handlers[i].HandlerAttribute.HandlerName ?? handlers[i].HandlerType.FullName }");
                _logger.LogInformation($"上次触发时间：{ handlers[i].LastAccessTime } 触发间隔：{ handlers[i].HandlerAttribute.Interval }");
                handlers[i].LastAccessTime = e.SignalTime;
                RunHandler(handlers[i]);
            }
        }

        private async void RunHandler(TimedHandlerItem handler)
        {
            using var sp = _serviceProvider.CreateScope();
            try
            {
                await ((ITimedHandler)sp.ServiceProvider.GetRequiredService(handler.HandlerType.AsType()))?.DoHandle(handler.HandlerAttribute.Interval, handler.LastAccessTime);
            }
            catch (Exception ex)
            {
                _logger.LogError($"定时处理器{ handler.HandlerAttribute.HandlerName ?? handler.HandlerType.FullName }发生未经处理的异常：\n{ ex.Message }\n堆栈追踪：\n{ ex.StackTrace }");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var handlers = _handlers.Where(h => h.HandlerAttribute.RunAfterStartup).ToList();
            _timer.Start();
            if (handlers.Count == 0)
            {
                for (int i = 0; i < handlers.Count; i++)
                {
                    _logger.LogInformation($"因应用程序启动已触发定时处理器：{ handlers[i].HandlerAttribute.HandlerName ?? handlers[i].HandlerType.FullName }");
                    handlers[i].LastAccessTime = DateTime.Now;
                    RunHandler(handlers[i]);
                }
            }
            return Task.CompletedTask;
        }

        private class TimedHandlerItem
        {
            public DateTime LastAccessTime { get; set; }

            public TimedHandlerAttribute HandlerAttribute { get; }

            public TypeInfo HandlerType { get; }

            public TimedHandlerItem(DateTime lastAccessTime, TimedHandlerAttribute attribute, TypeInfo type)
            {
                LastAccessTime = lastAccessTime;
                HandlerAttribute = attribute;
                HandlerType = type;
            }
        }
    }
}