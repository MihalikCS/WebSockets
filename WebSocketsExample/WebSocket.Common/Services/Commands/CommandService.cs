using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebSocketCommon.Interfaces;
using WebSocketCommon.Services.Data;
using WebSocketServer.Model;

namespace WebSocketCommon.Services.Commands
{
    public class CommandService : ICommandService,
        ICommandHandler<GetNamesCommand>
    {
        ILogger _Logger { get; set; }
        IDataService _DataService { get; set; }

        public CommandService(IServiceProvider serviceProvider)
        {
            _Logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<CommandService>();
            _DataService = serviceProvider.GetRequiredService<IDataService>();
        }

        public async Task<CommandResponse> InvokeCommandAsync(Command cmd)
        {
            CommandResponse cmdResponse = new CommandResponse();
            cmdResponse.CommandId = cmd.CommandId;
            // Quick and really dirty implementation
            if (string.Compare(cmd.CommandType, "GetNames", true) == 0)
            {
                var dbCommand = _DataService.CreateCommand("SELECT * FROM Names");
                List<object[]> values = await _DataService.ExecuteReaderAsync(dbCommand);
                cmdResponse.Data = values.Select(a => string.Join(" ", a.Skip(1))).ToArray();
            }
            return cmdResponse;
        }

        public void Execute(GetNamesCommand cmd)
        {
            throw new NotImplementedException();
        }
    }
}
