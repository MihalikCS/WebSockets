using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace WebSocketServer.Model
{
    [DataContract]
    public class Command
    {
        [DataMember]
        public int CommandId { get; set; }

        [DataMember]
        public string CommandType { get; set; }
    }

    [DataContract]
    public sealed class CommandResponse
    {
        [DataMember]
        public int CommandId { get; set; }

        [DataMember]
        public string[] Data { get; set; }
    }

    //
    public class GetNamesCommand : Command { }; 

}
