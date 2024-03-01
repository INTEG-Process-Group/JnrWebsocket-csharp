using Integpg.JniorWebSocket;
using Integpg.JniorWebSocket.Messages;
using System;

namespace WebSocketExample.Commands
{
    public class RegistryIngestCommand : TelnetCommand
    {
        public RegistryIngestCommand(JniorWebSocket jniorWebsocket, string remoteFilename) : base(jniorWebsocket, "registry -i " + remoteFilename)
        {
        }



        protected override void DoCommand()
        {
            try
            {
                OnLog("sending registry ingest command");
                base.DoCommand();
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

    }
}
