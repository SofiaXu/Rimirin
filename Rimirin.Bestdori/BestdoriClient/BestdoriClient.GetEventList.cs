using Rimirin.Bestdori.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rimirin.Bestdori
{
    public partial class BestdoriClient
    {
        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <returns>活动列表</returns>
        public async Task<Dictionary<int, GarupaEvent>> GetEventList()
        {
            try
            {
                using var response = await httpClient.GetAsync(ApiBaseUrl + $"events/all.4.json");
                response.EnsureSuccessStatusCode();
                return await JsonSerializer.DeserializeAsync<Dictionary<int, GarupaEvent>>(await response.Content.ReadAsStreamAsync(), jsonOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取活动封面路径
        /// </summary>
        /// <param name="eventId">活动Id</param>
        /// <returns>本地磁盘相对路径</returns>
        public async Task<Stream> GetEventBannerImagePath(string eventId)
        {
            try
            {
                using var response = await httpClient.GetAsync(ApiBaseUrl + $"jp/homebanner_rip/banner_event{eventId}.png");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}