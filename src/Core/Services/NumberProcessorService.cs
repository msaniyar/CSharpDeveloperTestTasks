using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class NumberProcessorService : INumberProcessorService
    {

        private readonly IHubConnectionBuilder _hubConnectionBuilder;
        private readonly ILogger<NumberProcessorService> _logger;
        private readonly List<bool> _messages = new();

        private const string ServerMethodName = "NumberProcessor";
        private const string ClientMethodName = "NumberProcessed";

        public NumberProcessorService(IHubConnectionBuilder hubConnectionBuilder, ILogger<NumberProcessorService> logger)
        {
            _hubConnectionBuilder = hubConnectionBuilder;
            _logger = logger;
        }


        /// <inheritdoc />
        public async Task<QueueServiceResponseModel> SendNumbers(int numbers)
        {

            if (numbers is <=0 or > 1000)
            {
                return new QueueServiceResponseModel
                {
                    Message = "Number should be in the range of 0..1000",
                    Success = false
                };
            }

            var connection = _hubConnectionBuilder.Build();
            connection?.StartAsync();

            connection?.On<bool>(ClientMethodName, async (response) =>
            {
                _messages.Add(response);
                _logger.LogInformation($"Message is processing... Current Count: {_messages.Count}");
                if (!_messages.Count.Equals(numbers)) return;
                _logger.LogInformation($"{numbers} messages are processed.");
                _messages.Clear();
                numbers = 0;
                await connection.DisposeAsync(); // When processing is done, dispose the connection.
            });

            if(connection is null)
            {
                return new QueueServiceResponseModel
                {
                    Message = "Connection unexpectedly closed.",
                    Success = false
                };
            }

            for (int value = 0; value < numbers; value++)
            {
                // Send numbers to the processor.
                await connection.SendAsync(ServerMethodName, value);

            }

            return new QueueServiceResponseModel
            {
                Message = "Data is successfully taken, It is being processed",
                Success = true
            };

        }
    }
}
