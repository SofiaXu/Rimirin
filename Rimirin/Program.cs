using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mirai_CSharp;
using Rimirin.Common;
using Rimirin.Garupa;
using Rimirin.Options;

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
                    services.AddSingleton<MiraiHttpSession>();
                    services.AddSingleton<GarupaData>();
                    services.AddMiraiPlugins();
                    services.AddRimiHandlers();
                    services.AddSingleton<BestdoriClient>();
                    services.AddSingleton<GachaImageRender>();
                    services.Configure<MiraiSessionOptions>(hostContext.Configuration.GetSection(MiraiSessionOptions.MiraiSession));
                });
    }
}