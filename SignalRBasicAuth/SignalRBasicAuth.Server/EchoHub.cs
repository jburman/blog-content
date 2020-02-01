using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SignalRBasicAuth.Server
{
    [Authorize]
    public class EchoHub : Hub
    {
        private ILogger<EchoHub> _logger;

        public EchoHub(ILogger<EchoHub> logger)
        {
            _logger = logger;
        }

        public async Task Connect(string fromUser)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", fromUser, "Hello - your userId is: " + 
                this.Context.UserIdentifier);
        }

        public async Task SendUser(string fromUser, string toUser, string message)
        {
            await Clients.User(toUser).SendAsync("ReceiveMessage", fromUser, message);
        }

        public async Task SendAll(string fromUser, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", fromUser, message);
        }
    }
}
