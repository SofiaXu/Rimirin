using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mirai_CSharp;
using Rimirin.Framework.DependencyInjection;
using Rimirin.Framework.Options;
using Rimirin.Garupa;

namespace Rimirin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddLogging();
                    services.AddMessageHandlers();
                    services.AddHelpHandler();
                    services.AddAutoReconnector();
                    services.AddMessageRouter();
                    services.AddSingleton<GarupaData>();
                    services.AddMiraiPlugins();
                    services.AddMirai(hostContext.Configuration.GetSection(SessionOptions.MiraiSession));
                    services.AddSingleton<BestdoriClient>();
                    services.AddSingleton<GachaImageRender>();
                });
    }
}