using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using Newtonsoft.Json;
using WebSocketServer.Model;

namespace WebSocketCommon.Services.Socket
{
    public sealed class SocketService : ISocketService
    {
        ILogger _Logger { get; set; }

        public SocketService(IServiceProvider serviceProvider)
        {
            _Logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<SocketService>();
        }

        public async Task<string> RecieveMessageAsync(System.Net.WebSockets.WebSocket socket, CancellationToken token)
        {
            var buffer = new byte[1024 * 4];
            using (_Logger.BeginScope($"Recieving message from socket with state {socket.State}", socket))
            {
                WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
            }
            return Encoding.UTF8.GetString(buffer);
        }

        public async Task<bool> SendMessageAsync(string message, System.Net.WebSockets.WebSocket socket, CancellationToken token)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            using (_Logger.BeginScope($"Sending message from socket with state {socket.State}", socket))
            {
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, token);
            }
            return true;
        }

        public async Task<T> RecieveItemAsync<T>(System.Net.WebSockets.WebSocket socket, CancellationToken token)
        {
            var buffer = new byte[1024 * 4];
            using (_Logger.BeginScope($"Recieving message from socket with state {socket.State}", socket))
            {
                WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
            }
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(buffer));
        }

        public async Task<bool> SendItemAsync<T>(T command, System.Net.WebSockets.WebSocket socket, CancellationToken token)
        {
            var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command));
            using (_Logger.BeginScope($"Sending message from socket with state {socket.State}", socket))
            {
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, token);
            }
            return true;
        }
    }
}
