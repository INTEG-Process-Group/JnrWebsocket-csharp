using Integpg.JniorWebSocket;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WebSocketExample.Commands
{
    public class TelnetCommand : CommandBase
    {
        private static string promptPattern = @"(\/\w+)*>";
        private string _command;
        public string Response
        {
            get
            {
                var response = _response.ToString();
                response = response.Substring(response.IndexOf("\n") + 1);
                if (response.Contains("\n"))
                {
                    response = response.Substring(0, response.LastIndexOf("\n"));
                }
                response = response.Trim();
                return response;
            }
        }

        private StringBuilder _response = new StringBuilder();
        private bool _responseReceived = false;



        public TelnetCommand(JniorWebSocket jniorWebsocket, string command) : base(jniorWebsocket)
        {
            _jniorWebsocket.ConsoleSession.ConsoleMessageReceived += ConsoleSession_MessageReceived;
            _command = command;
        }



        protected override void CommandFinished()
        {
            _jniorWebsocket.ConsoleSession.ConsoleMessageReceived -= ConsoleSession_MessageReceived;
        }



        protected override void DoCommand()
        {
            try
            {
                OnLog("send command: " + _command);
                _jniorWebsocket.ConsoleSession.Send(_command + "\r");

                var timeout = DateTime.Now.AddSeconds(15);
                while (!_responseReceived && DateTime.Now < timeout)
                    Thread.Sleep(50);
                if (DateTime.Now > timeout)
                    throw new Exception("Version response not recevied");
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }



        private void ConsoleSession_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                _response.Append(e.Message);
                Console.WriteLine(_response.Length);

                var match = Regex.Match(_response.ToString(), promptPattern);
                if (match.Success)
                {
                    _responseReceived = true;
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

    }
}
