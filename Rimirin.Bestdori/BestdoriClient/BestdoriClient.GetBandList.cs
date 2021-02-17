﻿using Rimirin.Bestdori.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rimirin.Bestdori
{
    public partial class BestdoriClient
    {
        /// <summary>
        /// 获取乐队列表
        /// </summary>
        /// <returns>角色列表</returns>
        public async Task<Dictionary<int, Band>> GetBandList()
        {
            try
            {
                using var response = await httpClient.GetAsync(ApiBaseUrl + $"bands/main.1.json");
                response.EnsureSuccessStatusCode();
                return await JsonSerializer.DeserializeAsync<Dictionary<int, Band>>(await response.Content.ReadAsStreamAsync(), jsonOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}