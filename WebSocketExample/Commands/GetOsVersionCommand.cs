using Integpg.JniorWebSocket;
using System;
using System.Text;
using System.Threading;

namespace WebSocketExample.Commands
{
    public class GetOsVersionCommand : TelnetCommand
    {
        public GetOsVersionCommand(JniorWebSocket jniorWebsocket) : base(jniorWebsocket, "reg $version")
        {
        }



        protected override void DoCommand()
        {
            try
            {
                OnLog("querying janos version");
                base.DoCommand();
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

    }
}
