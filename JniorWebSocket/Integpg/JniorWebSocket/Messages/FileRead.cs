using System;
using System.IO;

namespace Integpg.JniorWebSocket.Messages
{
    public class FileRead : MetaMessage
    {
        public FileRead(string remoteFileName) : base("File Read")
        {
            this["File"] = remoteFileName;
        }



        public FileRead(string remoteFileName, long requestId) : this(remoteFileName)
        {
            this["RequestID"] = requestId;
        }
    }
}
