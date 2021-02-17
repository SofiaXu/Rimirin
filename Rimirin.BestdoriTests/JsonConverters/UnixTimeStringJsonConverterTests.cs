using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rimirin.Bestdori.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rimirin.Bestdori.JsonConverters.Tests
{
    [TestCategory("Json转换测试")]
    [TestClass()]
    public class UnixTimeStringJsonConverterTests
    {
        [TestCategory("Json读取测试")]
        [TestMethod("读取测试")]
        [DataTestMethod()]
        [DataRow("[null]", null, DisplayName = "空值读取测试")]
        [DataRow(@"[""1046631783000""]", "2003/3/3 3:03:03", DisplayName = "字符串读取测试")]
        [DataRow(@"[1046631783000]", "2003/3/3 3:03:03", DisplayName = "数字读取测试")]
        [DataRow(@"[]", null, DisplayName = "空读取测试")]
        public void ReadTest(string json, string time)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new UnixTimeStringJsonConverter());
            var times = JsonSerializer.Deserialize<List<DateTime?>>(json, options);
            if (time != null)
            {
                Assert.AreEqual(times[0].Value, DateTime.Parse(time));
            }
            else if (json == "[]")
            {
                Assert.AreEqual(times.Count, 0);
            }
            else
            {
                Assert.IsNull(times[0]);
            }
        }

        [TestCategory("Json写入测试")]
        [TestMethod("写入测试")]
        [DataTestMethod()]
        [DataRow("[null]", null, DisplayName = "空值写入测试")]
        [DataRow(@"[""1046631783000""]", "2003/3/3 3:03:03", DisplayName = "字符串写入测试")]
        [DataRow(@"[]", null, DisplayName = "空读取测试")]
        public void WriteTest(string json, string time)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new UnixTimeStringJsonConverter());
            DateTime?[] times = new DateTime?[] { time == null ? null : DateTime.Parse(time) };
            if (json == "[]")
            {
                times = Array.Empty<DateTime?>();
            }
            var outJson = JsonSerializer.Serialize(times, options);
            if (time != null)
            {
                Assert.AreEqual(json, outJson);
            }
        }
    }
}