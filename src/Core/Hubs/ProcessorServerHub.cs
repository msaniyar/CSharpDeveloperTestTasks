using Microsoft.AspNetCore.SignalR;

namespace Core.Hubs
{
    public class ProcessorServerHub : Hub
    {
        public Task NumberProcessor(int number)
        {
            Thread.Sleep(100);
            return Clients.All.SendAsync("NumberProcessed", true);
        }
    }
}
