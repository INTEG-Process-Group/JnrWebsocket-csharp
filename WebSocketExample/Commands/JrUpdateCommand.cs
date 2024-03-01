using Integpg.JniorWebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace WebSocketExample.Commands
{
    public class JrUpdateCommand : CommandBase
    {
        private string _filename;
        private bool _rebooted = false;



        public JrUpdateCommand(JniorWebSocket jniorWebsocket, string filename) : base(jniorWebsocket)
        {
            _jniorWebsocket.Websocket.MessageReceived += Websocket_MessageReceived;
            _jniorWebsocket.Disconnected += JniorWebsocket_Disconnected;

            _filename = filename;
        }



        protected override void CommandFinished()
        {
            _jniorWebsocket.Websocket.MessageReceived -= Websocket_MessageReceived;
            _jniorWebsocket.Disconnected -= JniorWebsocket_Disconnected;
        }



        protected override void DoCommand()
        {
            try
            {
                OnLog("sending jrupdate command");
                _jniorWebsocket.ConsoleSession.Send("jrupdate -fup /temp/" + _filename + "\r");

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
            catch (Exception ex)
            {

            }
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



        private void JniorWebsocket_Disconnected(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

    }
}
