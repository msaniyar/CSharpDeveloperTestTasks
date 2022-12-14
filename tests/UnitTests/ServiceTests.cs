using Core.Hubs;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace UnitTests
{
    public class ServiceTests
    {

        private readonly IStringReverseService _stringReverseService;
        private readonly IFileHashService _fileHashService;

        private const string InitialString = "";
        private const string ExpectedReverseString = "";


        public ServiceTests()
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

    }
}
