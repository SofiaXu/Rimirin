using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin;
using Rimirin.Framework.Options;

namespace Rimirin.Framework.DependencyInjection
{
    public static partial class InjectionHelper
    {
        /// <summary>
        /// 注入Mirai-CSharp
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMirai(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<SessionOptions>(config);
            services.AddSingleton(sp =>
            {
                var miraiSessionOptions = sp.GetService<IOptions<SessionOptions>>();
                MiraiHttpSessionOptions options = new MiraiHttpSessionOptions(miraiSessionOptions.Value.MiraiHost, miraiSessionOptions.Value.MiraiHostPort, miraiSessionOptions.Value.MiraiSessionKey);
                var session = new MiraiHttpSession();
                var services = sp.GetServices<IPlugin>();
                foreach (var item in services)
                {
                    session.AddPlugin(item);
                }
                session.ConnectAsync(options, miraiSessionOptions.Value.MiraiSessionQQ).Wait();
                return session;
            });
            return services;
        }
    }
}