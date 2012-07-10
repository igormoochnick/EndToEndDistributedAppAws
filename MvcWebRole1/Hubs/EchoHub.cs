using SignalR.Hubs;

namespace MvcWebRole1.Hubs
{
    public class EchoHub : Hub
    {
        public void DoUpdateMessage(string message, string sender)
        {
            this.Clients.messageUpdated(message, sender);
        }

        public void DoHeartBeat()
        {
            this.Caller.heartBeat();
        }
    }
}