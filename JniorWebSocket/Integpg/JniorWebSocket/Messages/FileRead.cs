using System;
using System.IO;

namespace Integpg.JniorWebSocket.Messages
{
    public class FileRead : JniorMessage
    {
        public FileRead(string remoteFileName) : base("File Read")
        {
            this["File"] = remoteFileName;
            this["Meta"] = new MetaMessage();
        }



        public FileRead(string remoteFileName, long requestId) : this(remoteFileName)
        {
            this["RequestID"] = requestId;
        }
    }
}
