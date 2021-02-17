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
        [TestMethod("获取卡片列表成功")]
        public void GetCardListSuccessTest()
        {
            var list = bestdoriClient.GetCardList().GetAwaiter().GetResult();
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod("获取卡片列表失败网络原因")]
        public void GetCardListFailNetworkIssueTest()
        {
            mockHandler.Reset();
            mockHandler.SetupAnyRequest().ReturnsResponse(System.Net.HttpStatusCode.NotFound);
            Assert.ThrowsException<HttpRequestException>(() => { mockBestdoriClient.GetCardList().GetAwaiter().GetResult(); });
        }

        [TestMethod("获取卡片列表失败Json解析错误")]
        public void GetCardListFailJsonIssueTest()
        {
            mockHandler.Reset();
            mockHandler.SetupAnyRequest().ReturnsResponse(System.Net.HttpStatusCode.OK, content: "{", mediaType: "text/json");
            Assert.ThrowsException<JsonException>(() => { mockBestdoriClient.GetCardList().GetAwaiter().GetResult(); });
        }
    }
}