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
    [TestClass()]
    public partial class BestdoriClientTests : IDisposable
    {
        private readonly BestdoriClient mockBestdoriClient;
        private readonly BestdoriClient bestdoriClient;
        private readonly Mock<HttpMessageHandler> mockHandler;
        private bool disposedValue;

        public BestdoriClientTests()
        {
            mockHandler = new Mock<HttpMessageHandler>();
            mockBestdoriClient = new BestdoriClient(mockHandler.CreateClient(), false);
            bestdoriClient = new BestdoriClient();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    bestdoriClient.Dispose();
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