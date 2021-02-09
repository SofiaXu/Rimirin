using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace Rimirin.Common
{
    public static class MiraiPluginInjectExtensions
    {
        /// <summary>
        /// 插件注入
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMiraiPlugins(this IServiceCollection services)
        {
            var plugins = Assembly.GetExecutingAssembly().DefinedTypes.Where(ti => ti.GetInterface("IPlugin") != null && ti.IsClass == true).ToArray();
            foreach (var plugin in plugins)
            {
                services.AddSingleton(plugin.AsType());
            }
            return services;
        }

        /// <summary>
        /// 处理器注入
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRimiHandlers(this IServiceCollection services)
        {
            var plugins = Assembly.GetExecutingAssembly().DefinedTypes.Where(ti => ti.GetInterface("IHandler") != null && ti.IsClass == true).ToArray();
            foreach (var plugin in plugins)
            {
                services.AddSingleton(plugin.AsType());
            }
            return services;
        }
    }
}