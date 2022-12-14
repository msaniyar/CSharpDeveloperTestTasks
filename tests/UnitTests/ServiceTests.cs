using Core.Hubs;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests
{
    public class ServiceTests
    {

        private readonly IStringReverseService _stringReverseService;
        private readonly IFileHashService _fileHashService;
        private readonly INumberProcessorService _numberProcessorService;
        private Mock<IHubConnectionBuilder> _mockHubConnectionBuilder;
        private Mock<ILogger<NumberProcessorService>> _mockLogger;


        private const string InitialString = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor";
        private const string ExpectedReverseString = "ropmet domsuie od des ,tile gnicsipida rutetcesnoc ,tema tis rolod muspi meroL";


        public ServiceTests()
        {
            _stringReverseService = new StringReverseService();
            _fileHashService = new FileHashService();

            _mockHubConnectionBuilder = new Mock<IHubConnectionBuilder>();
            _mockLogger = new Mock<ILogger<NumberProcessorService>>();

            _numberProcessorService = new NumberProcessorService(_mockHubConnectionBuilder.Object, _mockLogger.Object);
        }

        [Fact]
        public void ReverseStringTest()
        {
            var reversedString = _stringReverseService.ReverseString(InitialString);
            Assert.Equal(ExpectedReverseString, reversedString);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void NullorEmptyReverseStringTest(string initialString)
        {
            var reversedString = _stringReverseService.ReverseString(initialString);
            Assert.Equal("", reversedString);
        }

        [Fact]
        public async Task ProcessorServerHubTest()
        {
            // Arrange
            var mockClients = new Mock<IHubCallerClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            var serverHub = new ProcessorServerHub()
            {
                Clients = mockClients.Object
            };

            // Act
            await serverHub.NumberProcessor(1);

            // Assert
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

        [Fact]
        public void FileNotFoundHashTest()
        {
            var filePath = "not/exist/file.txt";


            var calculatedHash = _fileHashService.CalculateFileHash(filePath);

            Assert.Equal(String.Empty, calculatedHash);

        }



        [Fact]
        public void GetPricesTest()
        {
            // Arrange
            var graphQlClient = new Mock<IGraphQLClient>();

            var prices = new Prices
            {
                Markets = new List<Market>
                {
                  new Market
                  {
                      MarketSymbol = "BTC",
                      Ticker = new Ticker
                      {
                          LastPrice = "3326.33000000"
                      }
                  }
                }
            };

            var asset = new Asset
            {
                AssetName = "Bitcoin",
                AssetSymbol = "BTC",
                MarketCap = 341624025248

            };

            var assets = new List<Asset>();

            for (int i = 0; i < 200; i++)
            {
                assets.Add(asset);
            }

            var pageAssets = new PageAssets
            {
                Assets = assets
            };


            var responsePrices = new GraphQLResponse<Prices>
            {
                Data = prices
            };

            var responsePageAssets = new GraphQLResponse<PageAssets>
            {
                Data = pageAssets
            };

            graphQlClient.
                Setup(client => client.SendQueryAsync<Prices>(It.IsAny<GraphQLRequest>(), CancellationToken.None)).Returns(Task.FromResult(responsePrices));
            graphQlClient.
                Setup(client => client.SendQueryAsync<PageAssets>(It.IsAny<GraphQLRequest>(), CancellationToken.None)).Returns(Task.FromResult(responsePageAssets));


            // Act
            var service = new PriceGetterService(graphQlClient.Object);
            var response = service.GetPrices();

            // Assert
            graphQlClient.Verify(client => client.SendQueryAsync<PageAssets>(It.IsAny<GraphQLRequest>(), CancellationToken.None), Times.Once);
            graphQlClient.Verify(client => client.SendQueryAsync<Prices>(It.IsAny<GraphQLRequest>(), CancellationToken.None), Times.Exactly(5));



        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1001)]
        public async Task NumberProcessorOverAndUnderLimitTest(int numbers)
        {

            var failedResponse = new QueueServiceResponseModel
            {
                Message = "Number should be in the range of 0..1000",
                Success = false
            };

            var resultBeyondLimit = await _numberProcessorService.SendNumbers(numbers);

            resultBeyondLimit.Should().BeEquivalentTo(failedResponse);

        }

        [Fact]
        public async Task NumberProcessorTest()
        {

            // Arrange
            const int numbers = 100;
            const string serverMethodName = "NumberProcessor";

            var successfulResponse = new QueueServiceResponseModel
            {
                Message = "Data is successfully taken, It is being processed",
                Success = true
            };

            var mockConnectionFactory = new Mock<IConnectionFactory>();
            var mockHubProtocol = new Mock<IHubProtocol>();
            var mockEndPoint = new Mock<EndPoint>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockConnection = new Mock<TestHubConnection>(
                mockConnectionFactory.Object,
                mockHubProtocol.Object,
                mockEndPoint.Object,
                mockServiceProvider.Object,
                mockLoggerFactory.Object
                );

            _mockHubConnectionBuilder.Setup(h => h.Build()).Returns(mockConnection.Object);


            // Act
            var resultSuccess = await _numberProcessorService.SendNumbers(numbers);

            // Assert
            resultSuccess.Should().BeEquivalentTo(successfulResponse);
            mockConnection.Verify(m => m.SendCoreAsync(serverMethodName, It.IsAny<object[]>(), CancellationToken.None), Times.Exactly(numbers));

        }

    }
}
