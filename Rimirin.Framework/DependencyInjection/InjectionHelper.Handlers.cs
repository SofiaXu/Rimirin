using Microsoft.Extensions.DependencyInjection;
using Rimirin.Framework.Options;
using System.Linq;
using System.Reflection;

namespace Rimirin.Framework.DependencyInjection
{
    public static partial class InjectionHelper
    {
        /// <summary>
        /// 注入消息处理器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
        {
            var plugins = Assembly.GetCallingAssembly().DefinedTypes.Where(ti => ti.GetInterface("IMessageHandler") != null && ti.IsClass == true && ti.IsAbstract == false).ToArray();
            foreach (var plugin in plugins)
            {
                services.AddScoped(plugin.AsType());
            }
            services.Configure<HandlerOptions>(o => o.Handlers.AddRange(plugins));
            return services;
        }

        /// <summary>
        /// 注入定时处理器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTimeHandlers(this IServiceCollection services)
        {
            var plugins = Assembly.GetCallingAssembly().DefinedTypes.Where(ti => ti.GetInterface("ITimeHandler") != null && ti.IsClass == true && ti.IsAbstract == false).ToArray();
            foreach (var plugin in plugins)
            {
                services.AddScoped(plugin.AsType());
            }
            services.Configure<HandlerOptions>(o => o.Handlers.AddRange(plugins));
            return services;
        }
    }
}