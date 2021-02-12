using Microsoft.Extensions.DependencyInjection;
using Mirai_CSharp.Plugin;
using Rimirin.Framework.Plugins;
using System.Linq;
using System.Reflection;

namespace Rimirin.Framework.DependencyInjection
{
    public static partial class InjectionHelper
    {
        /// <summary>
        /// 注入<see cref="Mirai_CSharp.Plugin.IPlugin"/>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMiraiPlugins(this IServiceCollection services)
        {
            var plugins = Assembly.GetCallingAssembly().DefinedTypes.Where(ti => ti.GetInterface("IPlugin") != null && ti.IsClass == true && ti.IsAbstract == false && ti.Namespace != "Rimirin.Framework.Plugins").ToArray();
            foreach (var plugin in plugins)
            {
                services.AddSingleton(typeof(IPlugin), plugin.AsType());
            }
            return services;
        }

        /// <summary>
        /// 注入消息路由器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMessageRouter(this IServiceCollection services)
        {
            services.AddSingleton<IPlugin, MessageRouterPlugin>();
            return services;
        }
        /// <summary>
        /// 注入自动重新连接器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAutoReconnector(this IServiceCollection services)
        {
            services.AddSingleton<IPlugin, ReconnectorPlugin>();
            return services;
        }
    }
}