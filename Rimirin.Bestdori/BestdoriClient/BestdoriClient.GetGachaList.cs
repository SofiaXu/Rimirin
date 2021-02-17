using Rimirin.Bestdori.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rimirin.Bestdori
{
    public partial class BestdoriClient
    {
        /// <summary>
        /// 获取卡池列表
        /// </summary>
        /// <returns>卡池列表</returns>
        public async Task<Dictionary<int, Gacha>> GetGachaList()
        {
            try
            {
                using var response = await httpClient.GetAsync(ApiBaseUrl + $"gacha/all.5.json");
                response.EnsureSuccessStatusCode();
                return await JsonSerializer.DeserializeAsync<Dictionary<int, Gacha>>(await response.Content.ReadAsStreamAsync(), jsonOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}