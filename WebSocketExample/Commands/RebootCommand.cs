using Integpg.JniorWebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace WebSocketExample.Commands
{
    public class RebootCommand : CommandBase
    {
        private bool _rebooted = false;



        public RebootCommand(JniorWebSocket jniorWebsocket) : base(jniorWebsocket)
        {
            _jniorWebsocket.Disconnected += JniorWebsocket_Disconnected;
            _jniorWebsocket.Websocket.MessageReceived += Websocket_MessageReceived;
        }



        protected override void CommandFinished()
        {
            _jniorWebsocket.Disconnected -= JniorWebsocket_Disconnected;
            _jniorWebsocket.Websocket.MessageReceived -= Websocket_MessageReceived;
        }



        private void Websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var json = JObject.Parse(e.Message);
                if (null != json["Data"])
                {
                    OnLog(json["Data"].ToString());
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }



        protected override void DoCommand()
        {
            OnLog("sending reboot command");
            _jniorWebsocket.ConsoleSession.Send("reboot -f\r");
            _rebooted = false;

            var timeout = DateTime.Now.AddSeconds(30);
            while (!_disconnected && DateTime.Now < timeout)
                Thread.Sleep(50);
            if (DateTime.Now > timeout)
                throw new Exception("JNIOR has not rebooted");
            OnLog("Rebooting...");

            timeout = DateTime.Now.AddSeconds(60);
            while (!_rebooted && DateTime.Now < timeout)
                Thread.Sleep(50);
            if (DateTime.Now > timeout)
                throw new Exception("JNIOR has not booted");
            OnLog("Rebooted");
        }



        private void JniorWebsocket_Disconnected(object sender, EventArgs e)
        {
            var disconnectedTime = DateTime.Now;
            var jniorWebSocket = sender as JniorWebSocket;
            if (!jniorWebSocket.Closing)
            {
                Thread.Sleep(10000);

                // try to reconnect
                for (int i = 0; i < 15; i++)
                {
                    OnLog("Try to reconnect");
                    jniorWebSocket.Reconnect();

                    if (jniorWebSocket.IsOpened)
                    {
                        // wait till we are authenticated
                        while (!jniorWebSocket.IsAuthenticated)
                        {
                            Thread.Sleep(50);
                        }

                        _rebooted = true;
                        var elapsed = DateTime.Now - disconnectedTime;
                        OnLog(string.Format("Took {0:0.0} seconds to reboot", elapsed.TotalSeconds));

                        break;
                    }
                }
            }
        }

    }
}
