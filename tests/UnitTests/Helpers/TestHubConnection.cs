using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using System.Net;


namespace UnitTests.Helpers
{
    public class TestHubConnection : HubConnection
    {
        public TestHubConnection(IConnectionFactory connectionFactory, IHubProtocol protocol, EndPoint endPoint, IServiceProvider serviceProvider, ILoggerFactory loggerFactory) : base(connectionFactory, protocol, endPoint, serviceProvider, loggerFactory)
        {
        }


        public override Task SendCoreAsync(string methodName, object?[] args, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public override Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public override IDisposable On(string methodName, Type[] parameterTypes, Func<object?[], object, Task> handler, object state)
        {
            return new Subscription();
        }


    }

    public class Subscription : IDisposable
    {

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
