using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class NumberProcessorService : INumberProcessorService
    {

        private readonly HubConnection? _connection;
        private readonly ILogger<NumberProcessorService> _logger;
        private readonly List<bool> _messages = new();

        private const string MethodName = "NumberProcessed";

        public NumberProcessorService(string url, ILogger<NumberProcessorService> logger)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();
            _connection.StartAsync();
            _logger = logger;
        }


        /// <inheritdoc />
        public async Task<QueueServiceResponseModel> SendNumbers(int numbers)
        {
            if(numbers is <=0 or > 1000)
            {
                if (_connection != null) await _connection.DisposeAsync();
                return new QueueServiceResponseModel
                {
                    Message = "Number should be in the range of 0..1000",
                    Success = false
                };
            }

            _connection?.On<bool>(MethodName, async (response) =>
            {
                _messages.Add(response);
                _logger.LogInformation($"Message is processing... Current Count: {_messages.Count}");
                if (!_messages.Count.Equals(numbers)) return;
                _logger.LogInformation($"{numbers} messages are processed.");
                _messages.Clear();
                numbers = 0;
                await _connection.DisposeAsync(); // When processing is done, dispose the connection.
            });

            if(_connection is null)
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
                await _connection.SendAsync("NumberProcessor", value);

            }

            return new QueueServiceResponseModel
            {
                Message = "Data is successfully taken, It is being processed",
                Success = true
            };

        }
    }
}
