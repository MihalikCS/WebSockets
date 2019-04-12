using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketCommon.Interfaces
{
    public interface ICommandHandler<Command>
    {
        void Execute(Command cmd);
    }
}
