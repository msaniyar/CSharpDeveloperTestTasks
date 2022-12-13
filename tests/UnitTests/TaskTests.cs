using Core.Hubs;
using Core.Interfaces;
using Core.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class TaskTests
    {

        private readonly IStringReverseService _stringReverseService;
        private readonly INumberProcessorService _numberProcessorService;
        private readonly IFileHashService _fileHashService;

        private const string InitialString = "";
        private const string ExpectedReverseString = "";


        public TaskTests()
        {
            _stringReverseService = new StringReverseService();
            _fileHashService = new FileHashService();
        }

        [Fact]
        public void ReverseStringTest()
        {
            var reversedString = _stringReverseService.ReverseString(InitialString);
            Assert.Equal(ExpectedReverseString, reversedString);
        }

        [Fact]
        public async Task ProcessorServerHubTest()
        {
            // arrange
            var mockClients = new Mock<IHubCallerClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            var serverHub = new ProcessorServerHub()
            {
                Clients = mockClients.Object
            };

            // act
            await serverHub.NumberProcessor(1);

            // assert
            mockClients.Verify(clients => clients.All, Times.Once);
            mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "NumberProcessed",
                    It.Is<object[]>(o => o.Length == 1 && o.FirstOrDefault()!.Equals(true)),
                    default(CancellationToken)),
                Times.Once);

        }

        [Fact]
        public void FileHashTest()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "testFile.bin");

            using FileStream fs = File.Create(filePath);
            fs.Dispose();

            var calculatedHash = _fileHashService.CalculateFileHash(filePath);

            Assert.Equal(
                "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", 
                calculatedHash);

            File.Delete(filePath);
        }

    }
}
