using Rimirin.Models.Garupa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rimirin.Garupa
{
    /// <summary>
    /// 简易BanG Dream!数据客户端，基于Bestdori
    /// </summary>
    public class BestdoriClient : IDisposable
    {
        protected HttpClient httpClient;
        public const string ApiBaseUrl = "https://bestdori.com/api/";
        public const string AssetsBaseUrl = "https://bestdori.com/assets/";
        /// <summary>
        /// 新建一个客户端
        /// </summary>
        public BestdoriClient()
        {
            httpClient = new HttpClient();
        }
        /// <summary>
        /// 确保文件夹存在（不存在将新建）
        /// </summary>
        /// <param name="path">路径</param>
        protected void EnsureDirectoryExists(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                di.Create();
            }
        }
        /// <summary>
        /// 下载活动列表
        /// </summary>
        /// <returns>成功则为<c>true</c>，否则为<c>false</c></returns>
        private async Task<bool> DownloadEvents()
        {
            var response = await httpClient.GetAsync(ApiBaseUrl + "events/all.4.json");
            if (response.IsSuccessStatusCode)
            {
                EnsureDirectoryExists("Resources\\Garupa\\Jsons");
                using FileStream file = new FileStream("Resources\\Garupa\\Jsons\\Events.json", FileMode.Create);
                await response.Content.CopyToAsync(file);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 下载活动封面
        /// </summary>
        /// <param name="eventId">活动Id</param>
        /// <returns>成功则为<c>true</c>，否则为<c>false</c></returns>
        private async Task<bool> DownloadEventBanner(string eventId)
        {
            var response = await httpClient.GetAsync(AssetsBaseUrl + $"jp/homebanner_rip/banner_event{eventId}.png");
            if (response.IsSuccessStatusCode)
            {
                EnsureDirectoryExists("Resources\\Garupa\\EventBanners");
                using FileStream file = new FileStream($"Resources\\Garupa\\EventBanners\\{eventId}.png", FileMode.Create);
                await response.Content.CopyToAsync(file);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <param name="update">是否通过网络更新</param>
        /// <returns>活动列表</returns>
        public async Task<Dictionary<string, GarupaEvent>> GetEvents(bool update = false)
        {
            if (update || !File.Exists("Resources\\Garupa\\Jsons\\Events.json"))
            {
                var result = await DownloadEvents();
                if (!result)
                {
                    throw new Exception("Cannot get events");
                }
            }

            using var oldFile = new FileStream("Resources\\Garupa\\Jsons\\Events.json", FileMode.Open);
            return await JsonSerializer.DeserializeAsync<Dictionary<string, GarupaEvent>>(oldFile, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
        }
        /// <summary>
        /// 获取活动封面路径
        /// </summary>
        /// <param name="eventId">活动Id</param>
        /// <param name="update">是否通过网络更新</param>
        /// <returns>本地磁盘相对路径</returns>
        public async Task<string> GetEventBannerImagePath(string eventId, bool update = false)
        {
            if (update || !File.Exists(@$"Resources\Garupa\EventBanners\{eventId}.png"))
            {
                var result = await DownloadEventBanner(eventId);
                if (!result)
                {
                    throw new Exception("Cannot get event banner");
                }
            }

            return $"Resources\\Garupa\\EventBanners\\{eventId}.png";
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    httpClient?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
