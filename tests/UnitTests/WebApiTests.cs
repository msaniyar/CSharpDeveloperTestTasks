using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Controllers;
using WebApi.Models;
using Xunit;
using FluentAssertions;

namespace UnitTests
{
    public class WebApiTests
    {

        private Mock<IStringReverseService> _stringReverseService  = new Mock<IStringReverseService>();
        private Mock<INumberProcessorService> _numberProcessorService = new Mock<INumberProcessorService>();
        private Mock<IFileHashService> _fileHashService = new Mock<IFileHashService>();
        private Mock<IPriceGetterService> _priceGetterService = new Mock<IPriceGetterService>();

        private readonly TaskController _taskController;


        public WebApiTests()
        {
            _taskController = new TaskController(
                _stringReverseService.Object,
                _numberProcessorService.Object,
                _fileHashService.Object,
                _priceGetterService.Object
                );
        }

        [Fact]
        public void GetReversedStringTest()
        {
            var initialString = "Lorem ipsum";
            var reverseString = "muspi moreL";

            _stringReverseService.Setup(s => s.ReverseString(initialString)).Returns(reverseString);
            var response = _taskController.GetReversedString(initialString);

            var okResponse = response as OkObjectResult;
            var stringResponse = okResponse?.Value as string;
            Assert.Equal(reverseString, stringResponse);
        }

        [Fact]
        public void GetReversedNullStringTest()
        {
            var initialString = "";
            var reverseString = "";

            _stringReverseService.Setup(s => s.ReverseString(initialString)).Returns(reverseString);
            var response = _taskController.GetReversedString(initialString);

            var badResponse = response as BadRequestObjectResult;
            Assert.Equal(400, badResponse?.StatusCode);
        }

        [Fact]
        public async Task ProcessNumbersTest()
        {
            var numbers = 5;
            var message = "Data is successfully taken, It is being processed";
            _numberProcessorService.Setup(s => s.SendNumbers(numbers)).
                ReturnsAsync(new QueueServiceResponseModel { Message = message, Success = true });

            var response = await _taskController.ProcessNumbers(new NumberProcessorRequestModel { Number = numbers});
            var okResponse = response as OkObjectResult;
            var stringResponse = okResponse?.Value as QueueServiceResponseModel;
            Assert.True(stringResponse?.Success);
            Assert.Equal(message, stringResponse?.Message);
        }

        [Fact]
        public async Task ProcessNumbersTestOverLimit()
        {
            var numbers = 1005;
            var message = "Number should be in the range of 0..1000";
            _numberProcessorService.Setup(s => s.SendNumbers(numbers)).
                ReturnsAsync(new QueueServiceResponseModel { Message = message, Success = false });

            var response = await _taskController.ProcessNumbers(new NumberProcessorRequestModel { Number = numbers });

            var badResponse = response as BadRequestObjectResult;
            Assert.Equal(400, badResponse?.StatusCode);

            var stringResponse = badResponse?.Value as QueueServiceResponseModel;
            Assert.False(stringResponse?.Success);
            Assert.Equal(message, stringResponse?.Message);
        }

        [Fact]
        public void GetFileHashTest()
        {
            var filePath = "path/to/file.txt";
            var hexResult = "GoodHexResult";

            _fileHashService.Setup(f => f.CalculateFileHash(filePath)).Returns(hexResult);

            var response = _taskController.GetFileHash(filePath);
            var okResponse = response as OkObjectResult;
            var stringResponse = okResponse?.Value as string;
            Assert.Equal(hexResult, stringResponse);

        }

        [Fact]
        public void GetFileDoesNotExistHashTest()
        {
            var filePath = "path/to/file.txt";

            _fileHashService.Setup(f => f.CalculateFileHash(filePath)).Returns("");

            var response = _taskController.GetFileHash(filePath);
            var notFoundResponse = response as NotFoundObjectResult;
            Assert.Equal(404, notFoundResponse?.StatusCode);

            var stringResponse = notFoundResponse?.Value as string;
            Assert.Equal("File does not exist.", stringResponse);
        }

        [Fact]
        public async Task GetPricesTest()
        {
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

            _priceGetterService.Setup(p => p.GetPrices()).ReturnsAsync(prices);

            var response = await _taskController.GetPrices();
            var okResponse = response as OkObjectResult;
            var pricesResponse = okResponse?.Value as Prices;

            pricesResponse.Should().BeEquivalentTo(prices);

        }


    }
}
