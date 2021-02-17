using Rimirin.Bestdori.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rimirin.Bestdori
{
    public partial class BestdoriClient
    {
        /// <summary>
        /// 获取指定卡池详细信息
        /// </summary>
        /// <param name="id">卡池编号</param>
        /// <returns>卡池列表</returns>
        public async Task<GachaDetail> GetGachaDetail(int id)
        {
            try
            {
                using var response = await httpClient.GetAsync(ApiBaseUrl + $"gacha/{id}.json");
                response.EnsureSuccessStatusCode();
                return await JsonSerializer.DeserializeAsync<GachaDetail>(await response.Content.ReadAsStreamAsync(), jsonOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取卡池封面
        /// </summary>
        /// <param name="resourceId">卡池资源Id</param>
        /// <param name="id">卡池Id</param>
        /// <returns>卡池封面</returns>
        public async Task<Stream> GetGachaBannerImagePath(string resourceId, int id)
        {
            try
            {
                using var response = await httpClient.GetAsync(ApiBaseUrl + $"jp/homebanner_rip/{resourceId}.png");
                if (!response.IsSuccessStatusCode)
                {
                    using var resp = await httpClient.GetAsync(ApiBaseUrl + $"jp/gacha/screen/gacha{id}_rip/logo.png");
                    resp.EnsureSuccessStatusCode();
                    return await resp.Content.ReadAsStreamAsync();
                }
                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}