using Integpg.JniorWebSocket;
using Integpg.JniorWebSocket.Messages;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace WebSocketExample.Commands
{
    public class FileListCommand : CommandBase
    {
        public string RemoteFolder { get; private set; }
        public JArray FileListing { get; private set; }

        private ManualResetEvent _fileListReceivedWait = new ManualResetEvent(false);



        public FileListCommand(JniorWebSocket jniorWebsocket, string remoteFolder) : base(jniorWebsocket)
        {
            RemoteFolder = remoteFolder;

            _jniorWebsocket.Websocket.MessageReceived += Websocket_MessageReceived;
        }



        private void Websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var json = JObject.Parse(e.Message);
                var message = json["Message"].ToString();
                if ("File List Response".Equals(message))
                {
                    var fileListContent = json["Content"].ToString();
                    FileListing = JArray.Parse(fileListContent);

                    _fileListReceivedWait.Set();
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }



        protected override void CommandFinished()
        {
            _jniorWebsocket.Websocket.MessageReceived -= Websocket_MessageReceived;
        }



        protected override void DoCommand()
        {
            try
            {
                OnLog(string.Format("reading {0}", RemoteFolder));

                // read the file
                var fileRead = new FileList(RemoteFolder);
                _jniorWebsocket.Send(fileRead);

                _fileListReceivedWait.WaitOne(10000);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

    }
}
