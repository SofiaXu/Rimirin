using Rimirin.Bestdori.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rimirin.Bestdori
{
    public partial class BestdoriClient
    {
        /// <summary>
        /// 获取指定角色详细信息
        /// </summary>
        /// <param name="id">角色Id</param>
        /// <returns>角色详情</returns>
        public async Task<CharacterDetail> GetCharacterDetail(int id)
        {
            try
            {
                var response = await httpClient.GetAsync(ApiBaseUrl + $"characters/{id}.json");
                response.EnsureSuccessStatusCode();
                return await JsonSerializer.DeserializeAsync<CharacterDetail>(await response.Content.ReadAsStreamAsync(), jsonOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}