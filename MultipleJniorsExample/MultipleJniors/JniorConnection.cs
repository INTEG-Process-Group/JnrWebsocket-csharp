using Integpg.JniorWebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MultipleJniors
{
    class JniorConnection
    {
        public JniorWebSocket WebSocket { get; private set; }
        public List<string> Comms = new List<string>();

        private Dictionary<string, EventHandler<MessageReceivedEventArgs>> _eventsByHash = new Dictionary<string, EventHandler<MessageReceivedEventArgs>>();



        public JniorConnection(ConnectionProperties connectionPropterties)
        {
            WebSocket = new JniorWebSocket(connectionPropterties.IpAddress, connectionPropterties.Port);
            WebSocket.IsSecure = connectionPropterties.IsSecure;

            WebSocket.MessageReceived += WebSocket_MessageReceived;
            WebSocket.MessageSent += WebSocket_MessageSent;
        }



        private void WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Comms.Add("RECV <- " + e.Message);

            var json = JObject.Parse(e.Message);
            if (null != json["Meta"] && null != json["Meta"]["Hash"])
            {
                var hash = (string)json["Meta"]["Hash"];
                var eventHandler = _eventsByHash[hash];
                if (null != eventHandler)
                {
                    eventHandler.Invoke(sender, e);
                }
            }
        }



        private void WebSocket_MessageSent(object sender, MessageSentEventArgs e)
        {
            Comms.Add("SENT -> " + e.Message);
        }



        public string GetRequestHash()
        {
            var l = (long)(new Random().NextDouble() * (1 << 32));
            return l.ToString("x8");
        }



        public void RegisterEventByHash(string hash, EventHandler<MessageReceivedEventArgs> eventHandler)
        {
            _eventsByHash.Add(hash, eventHandler);
        }
    }
}
