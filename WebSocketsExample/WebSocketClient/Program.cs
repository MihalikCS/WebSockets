using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using WebSocketCommon.Services.Data;
using WebSocketCommon.Services.Socket;
using WebSocketServer.Model;

namespace WebSocketClient
{
    public class Bootstrapper
    {
        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDataService, DataService>()
            .AddTransient<ISocketService, SocketService>()
            .AddLogging(options =>
            {
                options.AddConsole();
                options.SetMinimumLevel(LogLevel.Debug);
            });

            Console.WriteLine("Client Start");
            Application myApp = new Application(serviceCollection.BuildServiceProvider());
            myApp.Run();

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }

    public sealed class Application
    {
        ILogger _Logger { get; set; }
        IServiceProvider _Services { get; set; }
        ISocketService _SocketService { get; set; }

        public Application(IServiceProvider serviceProvider)
        {
            _Services = serviceProvider;
            _Logger = _Services.GetService<ILoggerFactory>().CreateLogger<Application>();
            _SocketService = _Services.GetRequiredService<ISocketService>();
        }

        public async void Run()
        {
            ClientWebSocket cws = new ClientWebSocket();
            await cws.ConnectAsync(new Uri("ws://localhost:5000/ws"), new CancellationToken());

            Command cmd = new Command()
            {
                CommandId = 3,
                CommandType = "GetNames"
            };

            await _SocketService.SendItemAsync<Command>(cmd, cws, new CancellationToken());
            var cmdResponse = await _SocketService.RecieveItemAsync<CommandResponse>(cws, new CancellationToken());
        }
    }
}
