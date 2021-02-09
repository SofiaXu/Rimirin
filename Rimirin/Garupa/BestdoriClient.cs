using Rimirin.Models.Garupa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
            return await JsonSerializer.DeserializeAsync<Dictionary<string, GarupaEvent>>(oldFile, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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

        /// <summary>
        /// 下载卡片列表
        /// </summary>
        /// <returns>成功则为<c>true</c>，否则为<c>false</c></returns>
        private async Task<bool> DownloadCards()
        {
            var response = await httpClient.GetAsync(ApiBaseUrl + "cards/all.5.json");
            if (response.IsSuccessStatusCode)
            {
                EnsureDirectoryExists("Resources\\Garupa\\Jsons");
                using FileStream file = new FileStream("Resources\\Garupa\\Jsons\\Cards.json", FileMode.Create);
                await response.Content.CopyToAsync(file);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取卡片列表
        /// </summary>
        /// <param name="update">是否通过网络更新</param>
        /// <returns>卡片列表</returns>
        public async Task<Dictionary<string, Card>> GetCards(bool update = false)
        {
            if (update || !File.Exists("Resources\\Garupa\\Jsons\\Cards.json"))
            {
                var result = await DownloadCards();
                if (!result)
                {
                    throw new Exception("Cannot get events");
                }
            }

            using var oldFile = new FileStream("Resources\\Garupa\\Jsons\\Cards.json", FileMode.Open);
            return await JsonSerializer.DeserializeAsync<Dictionary<string, Card>>(oldFile, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        /// <summary>
        /// 下载卡池列表
        /// </summary>
        /// <returns>成功则为<c>true</c>，否则为<c>false</c></returns>
        private async Task<bool> DownloadGacha()
        {
            var response = await httpClient.GetAsync(ApiBaseUrl + "gacha/all.5.json");
            if (response.IsSuccessStatusCode)
            {
                EnsureDirectoryExists("Resources\\Garupa\\Jsons");
                using FileStream file = new FileStream("Resources\\Garupa\\Jsons\\Gacha.json", FileMode.Create);
                await response.Content.CopyToAsync(file);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取卡池列表
        /// </summary>
        /// <param name="update">是否通过网络更新</param>
        /// <returns>卡池列表</returns>
        public async Task<Dictionary<string, Gacha>> GetGacha(bool update = false)
        {
            if (update || !File.Exists("Resources\\Garupa\\Jsons\\Gacha.json"))
            {
                var result = await DownloadGacha();
                if (!result)
                {
                    throw new Exception("Cannot get events");
                }
            }

            using var oldFile = new FileStream("Resources\\Garupa\\Jsons\\Gacha.json", FileMode.Open);
            return await JsonSerializer.DeserializeAsync<Dictionary<string, Gacha>>(oldFile, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        /// <summary>
        /// 下载角色列表
        /// </summary>
        /// <returns>成功则为<c>true</c>，否则为<c>false</c></returns>
        private async Task<bool> DownloadCharacters()
        {
            var response = await httpClient.GetAsync(ApiBaseUrl + "characters/main.3.json");
            if (response.IsSuccessStatusCode)
            {
                EnsureDirectoryExists("Resources\\Garupa\\Jsons");
                using FileStream file = new FileStream("Resources\\Garupa\\Jsons\\Characters.json", FileMode.Create);
                await response.Content.CopyToAsync(file);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="update">是否通过网络更新</param>
        /// <returns>角色列表</returns>
        public async Task<Dictionary<string, Character>> GetCharacters(bool update = false)
        {
            if (update || !File.Exists("Resources\\Garupa\\Jsons\\Characters.json"))
            {
                var result = await DownloadCharacters();
                if (!result)
                {
                    throw new Exception("Cannot get events");
                }
            }

            using var oldFile = new FileStream("Resources\\Garupa\\Jsons\\Characters.json", FileMode.Open);
            return await JsonSerializer.DeserializeAsync<Dictionary<string, Character>>(oldFile, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        /// <summary>
        /// 下载乐队列表
        /// </summary>
        /// <returns>成功则为<c>true</c>，否则为<c>false</c></returns>
        private async Task<bool> DownloadBands()
        {
            var response = await httpClient.GetAsync(ApiBaseUrl + "bands/main.1.json");
            if (response.IsSuccessStatusCode)
            {
                EnsureDirectoryExists("Resources\\Garupa\\Jsons");
                using FileStream file = new FileStream("Resources\\Garupa\\Jsons\\Bands.json", FileMode.Create);
                await response.Content.CopyToAsync(file);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取乐队列表
        /// </summary>
        /// <param name="update">是否通过网络更新</param>
        /// <returns>角色列表</returns>
        public async Task<Dictionary<string, Band>> GetBands(bool update = false)
        {
            if (update || !File.Exists("Resources\\Garupa\\Jsons\\Bands.json"))
            {
                var result = await DownloadBands();
                if (!result)
                {
                    throw new Exception("Cannot get events");
                }
            }

            using var oldFile = new FileStream("Resources\\Garupa\\Jsons\\Bands.json", FileMode.Open);
            return await JsonSerializer.DeserializeAsync<Dictionary<string, Band>>(oldFile, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        /// <summary>
        /// 下载指定卡池详细信息
        /// </summary>
        /// <returns>成功则为<c>true</c>，否则为<c>false</c></returns>
        private async Task<bool> DownloadGacha(string id)
        {
            var response = await httpClient.GetAsync(ApiBaseUrl + $"gacha/{id}.json");
            if (response.IsSuccessStatusCode)
            {
                EnsureDirectoryExists("Resources\\Garupa\\Jsons\\Gacha");
                using FileStream file = new FileStream($"Resources\\Garupa\\Jsons\\Gacha\\{id}.json", FileMode.Create);
                await response.Content.CopyToAsync(file);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取指定卡池详细信息
        /// </summary>
        /// <param name="update">是否通过网络更新</param>
        /// <returns>角色列表</returns>
        public async Task<GachaDetail> GetGacha(string id, bool update = false)
        {
            if (update || !File.Exists($"Resources\\Garupa\\Jsons\\Gacha\\{id}.json"))
            {
                var result = await DownloadGacha(id);
                if (!result)
                {
                    throw new Exception("Cannot get events");
                }
            }

            using var oldFile = new FileStream($"Resources\\Garupa\\Jsons\\Gacha\\{id}.json", FileMode.Open);
            return await JsonSerializer.DeserializeAsync<GachaDetail>(oldFile, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        /// <summary>
        /// 下载卡池封面
        /// </summary>
        /// <param name="resourceId">卡池资源Id</param>
        /// <returns>成功则为<c>true</c>，否则为<c>false</c></returns>
        private async Task<bool> DownloadGachaBanner(string resourceId)
        {
            var response = await httpClient.GetAsync(AssetsBaseUrl + $"jp/homebanner_rip/{resourceId}.png");
            if (response.IsSuccessStatusCode)
            {
                EnsureDirectoryExists("Resources\\Garupa\\GachaBanners");
                using FileStream file = new FileStream($"Resources\\Garupa\\GachaBanners\\{resourceId}.png", FileMode.Create);
                await response.Content.CopyToAsync(file);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取卡池封面路径
        /// </summary>
        /// <param name="resourceId">卡池Id</param>
        /// <param name="update">是否通过网络更新</param>
        /// <returns>本地磁盘相对路径</returns>
        public async Task<string> GetGachaBannerImagePath(string resourceId, bool update = false)
        {
            if (update || !File.Exists(@$"Resources\Garupa\GachaBanners\{resourceId}.png"))
            {
                var result = await DownloadGachaBanner(resourceId);
                if (!result)
                {
                    throw new Exception("Cannot get event banner");
                }
            }

            return $"Resources\\Garupa\\GachaBanners\\{resourceId}.png";
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