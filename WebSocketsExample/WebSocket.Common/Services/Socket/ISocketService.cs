using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebSocketServer.Model;

namespace WebSocketCommon.Services.Socket
{
    public interface ISocketService
    {
        Task<string> RecieveMessageAsync(System.Net.WebSockets.WebSocket socket, CancellationToken token);
        Task<bool> SendMessageAsync(string message, System.Net.WebSockets.WebSocket socket, CancellationToken token);
        Task<T> RecieveItemAsync<T>(System.Net.WebSockets.WebSocket socket, CancellationToken token);
        Task<bool> SendItemAsync<T>(T item, System.Net.WebSockets.WebSocket socket, CancellationToken token);
    }
}
