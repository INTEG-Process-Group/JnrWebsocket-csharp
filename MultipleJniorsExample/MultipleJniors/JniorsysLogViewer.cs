using Integpg.JniorWebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Windows.Forms;

namespace MultipleJniors
{
    class JniorsysLogViewer : TextBox
    {
        private JniorConnection _jniorConnection;



        public JniorsysLogViewer(JniorConnection jniorConnection)
        {
            _jniorConnection = jniorConnection;

            Dock = DockStyle.Fill;
            Multiline = true;
            ScrollBars = ScrollBars.Both;
        }



        public void ReadFile()
        {
            var requestHash = _jniorConnection.GetRequestHash();
            _jniorConnection.RegisterEventByHash(requestHash, new EventHandler<MessageReceivedEventArgs>(WebSocket_MessageReceived));
            var json = JObject.FromObject(new
            {
                Message = "File Read",
                File = "/jniorsys.log",
                Meta = new { Hash = requestHash }
            });
            _jniorConnection.WebSocket.Send(json);
        }


        private void WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var json = JObject.Parse(e.Message);
                var data = (string)json["Data"];
                var contents = Encoding.ASCII.GetString(Convert.FromBase64String(data));

                Invoke((MethodInvoker)delegate ()
                {
                    // our registered event has been called!
                    Text = contents;
                });
            }
            catch (Exception ex)
            {

            }
        }
    }
}
