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
        /// 获取卡片列表
        /// </summary>
        /// <returns>卡片列表</returns>
        public async Task<Dictionary<int, Card>> GetCardList()
        {
            try
            {
                using var response = await httpClient.GetAsync(ApiBaseUrl + $"cards/all.5.json");
                response.EnsureSuccessStatusCode();
                return await JsonSerializer.DeserializeAsync<Dictionary<int, Card>>(await response.Content.ReadAsStreamAsync(), jsonOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取卡片缩略图路径
        /// </summary>
        /// <param name="resourceId">卡池Id</param>
        /// <param name="cardId">卡片Id</param>
        /// <param name="isNormal">是否为普通卡面，默认普卡</param>
        /// <returns>卡片缩略图</returns>
        public async Task<Stream> GetCardThumbPath(string resourceId, int cardId, bool isNormal = true)
        {
            try
            {
                using var response = await httpClient.GetAsync(ApiBaseUrl + $"jp/thumb/chara/card{Math.Floor(cardId / 50d):00000}_rip/{resourceId}_{(isNormal ? "normal" : "after_training")}.png");
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