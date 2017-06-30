using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WEBAPP.WebSockets
{
    public class WebSocketHubHandler: WebSocketHandler
    {
        public WebSocketHubHandler(WebSocketConnectionManager webSocketConnectionManager, IServiceProvider serviceProvider) : base(webSocketConnectionManager, serviceProvider)
        {
        }

        public async Task Echo (string msg, WebSocket socket)
        {
            await InvokeClientMethodAsync(WebSocketConnectionManager.GetId(socket), "EchoCallback", new object[]{ msg });
        }
    }
}
