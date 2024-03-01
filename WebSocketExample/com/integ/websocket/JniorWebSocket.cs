using com.integpg.websocket.messages;
using Newtonsoft.Json.Linq;
//using SuperSocket.ClientEngine;
using System;
//using WebSocket4Net;

namespace com.integpg.websocket
{
    public class JniorWebSocket
    {
        public event EventHandler<LogEventArgs> Log;
        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<UnauthorizedEventArgs> Unauthorized;

        public string Host { get; private set; }
        public int Port { get; private set; }
        public string Uri { get; private set; }

        private WebSocket _websocket;

        public JniorWebSocket(string host) : this(host, 0) { }


        public JniorWebSocket(string host, int port)
        {
            Host = host;
            Port = port;
        }



        public bool IsOpened { get; private set; }



        public bool IsAuthenticated { get; private set; }



        public bool IsSecure { get; set; }



        public bool AllowUnstrustedCertificate { get; set; }



        public void Connect()
        {
            Uri = string.Format("{0}://{1}{2}", IsSecure ? "wss" : "ws", Host, (0 != Port) ? ":" + Port : "");

            _websocket = new WebSocket(Uri);
            _websocket.AllowUnstrustedCertificate = AllowUnstrustedCertificate;
            _websocket.Opened += new EventHandler(websocket_Opened);
            _websocket.Error += Error;
            _websocket.Closed += websocket_Closed;
            _websocket.MessageReceived += Websocket_MessageReceived;

            Log?.Invoke(this, new LogEventArgs("Connect to " + Uri + "\r\n"));
            _websocket.Open();
        }



        internal void Close()
        {
            _websocket.Close();
        }



        public void Send(JObject json)
        {
            try
            {
                Log?.Invoke(this, new LogEventArgs("SEND -> " + json.ToString(Newtonsoft.Json.Formatting.None) + "\r\n"));
                _websocket.Send(json.ToString());
            }
            catch (Exception ex)
            {
                Log?.Invoke(this, new LogEventArgs("\r\n" + ex.ToString() + "\r\n\r\n"));
            }
        }



        private void Websocket_Error(object sender, ErrorEventArgs e)
        {
            Log?.Invoke(this, new LogEventArgs("\r\n" + e.Exception.ToString() + "\r\n\r\n"));
        }



        private void Websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var json = JObject.Parse(e.Message);

            Log?.Invoke(this, new LogEventArgs("RECV <- " + json.ToString(Newtonsoft.Json.Formatting.None) + "\r\n"));

            var message = json["Message"].ToString();
            if ("Error".Equals(message)) HandleErrorMessage(json);
            else if ("Monitor".Equals(message)) IsAuthenticated = true;
            else if ("Authenticated".Equals(message)) IsAuthenticated = true;
        }



        private void HandleErrorMessage(JObject json)
        {
            var text = json["Text"].ToString();
            if ("401 Unauthorized".Equals(text))
            {
                var nonce = json["Nonce"].ToString();
                Unauthorized?.Invoke(this, new UnauthorizedEventArgs(nonce));
            }
        }



        private void websocket_Opened(object sender, EventArgs e)
        {
            IsOpened = true;
            Connected?.Invoke(this, EventArgs.Empty);
            Send(JniorMessage.Empty);
        }



        private void websocket_Closed(object sender, EventArgs e)
        {
            IsOpened = false;
            IsAuthenticated = false;
            Disconnected?.Invoke(this, EventArgs.Empty);
        }
    }
}