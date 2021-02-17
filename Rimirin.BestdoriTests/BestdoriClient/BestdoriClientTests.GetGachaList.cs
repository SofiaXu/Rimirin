using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Contrib.HttpClient;
using Rimirin.Bestdori;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rimirin.Bestdori.Tests
{
    public partial class BestdoriClientTests
    {
        [TestMethod("获取卡池列表成功")]
        public void GetGachaListSuccessTest()
        {
            var list = bestdoriClient.GetGachaList().GetAwaiter().GetResult();
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod("获取卡池列表失败网络原因")]
        public void GetGachaListFailNetworkIssueTest()
        {
            mockHandler.Reset();
            mockHandler.SetupAnyRequest().ReturnsResponse(System.Net.HttpStatusCode.NotFound);
            Assert.ThrowsExceptionAsync<HttpRequestException>(mockBestdoriClient.GetGachaList).GetAwaiter().GetResult();
        }

        [TestMethod("获取卡池列表失败Json解析错误")]
        public void GetGachaListFailJsonIssueTest()
        {
            mockHandler.Reset();
            mockHandler.SetupAnyRequest().ReturnsResponse(System.Net.HttpStatusCode.OK, content: "{", mediaType: "text/json");
            Assert.ThrowsExceptionAsync<JsonException>(mockBestdoriClient.GetGachaList).GetAwaiter().GetResult();
        }
    }
}