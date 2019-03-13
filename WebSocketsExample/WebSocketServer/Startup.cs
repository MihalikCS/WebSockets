using System;
using System.Net.WebSockets;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebSocketCommon.Services.Commands;
using WebSocketCommon.Services.Data;
using WebSocketCommon.Services.Socket;
using WebSocketServer.Model;

namespace WebSocketServer
{
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-2.2
    //
    public class Startup
    {
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure Services
            services.AddSingleton<IDataService, DataService>()
                    .AddSingleton<ICommandService, CommandService>()
                    .AddTransient<ISocketService, SocketService>()
                    .AddLogging(options =>
                    {
                        options.AddConsole();
                        options.SetMinimumLevel(LogLevel.Debug);
                    })
                    .AddMvc();
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            // Web Sockets Mapping
            app.Map("/ws", ws =>
            {
                var webSocketOptions = new WebSocketOptions()
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(120),
                    ReceiveBufferSize = 4 * 1024
                };
                ws.UseWebSockets(webSocketOptions)
                .Use(async (context, next) =>
                {
                    var socketService = app.ApplicationServices.GetRequiredService<ISocketService>();
                    var commandService = app.ApplicationServices.GetRequiredService<ICommandService>();

                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        System.Net.WebSockets.WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var command = await socketService.RecieveItemAsync<Command>(webSocket, new CancellationToken());
                        var commandResponse = await commandService.InvokeCommandAsync(command);
                        await socketService.SendItemAsync<CommandResponse>(commandResponse, webSocket, new CancellationToken());
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                });
            });

            // Rest Service Mapping
            app.Map("/api", rs =>
            {
                rs.UseMvc();
            });
        }
    }
}
