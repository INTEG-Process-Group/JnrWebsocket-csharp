﻿using Integpg.JniorWebSocket.Messages;
using Newtonsoft.Json.Linq;
using System;

namespace Integpg.JniorWebSocket
{
    /**
     * This class will be responsible for opening a console session.  It will send and receive 
     * messages as if it were a command line session
     */
    public class ConsoleSession
    {
        public event EventHandler<LogEventArgs> Log;
        public event EventHandler<ExceptionEventArgs> Error;
        public event EventHandler<MessageReceivedEventArgs> ConsoleMessageReceived;

        private JniorWebSocket _jniorWebSocket;



        public ConsoleSession(JniorWebSocket jniorWebSocket)
        {
            _jniorWebSocket = jniorWebSocket;

            _jniorWebSocket.Websocket.MessageReceived += Websocket_MessageReceived;
        }



        private void Websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.Message))
                    return;

                var json = JObject.Parse(e.Message);

                //Log?.Invoke(this, new LogEventArgs("RECV <- " + json.ToString(Newtonsoft.Json.Formatting.None) + "\r\n"));

                var message = json["Message"].ToString();
                if ("Console Response".Equals(message))
                {
                    var status = (string)json["Status"];
                    if ("Established".Equals(status))
                    {
                        //_consoleOpen = true;
                    }
                    else if ("Closed".Equals(status))
                    {
                        _jniorWebSocket.ConsoleOpen = false;
                    }
                }
                else if ("Console Stdout".Equals(message))
                {
                    var data = (string)json["Data"];
                    var meta = null as string;
                    var handled = false;

                    if (data.Contains(" login: "))
                    {
                        meta = "loginprompt";
                        _jniorWebSocket.MetaResponses[meta] = json;
                        handled = true;
                    }
                    else if (data.Contains(" password: "))
                    {
                        meta = "passwordprompt";
                        _jniorWebSocket.MetaResponses[meta] = json;
                        handled = true;
                    }
                    else if (data.Contains("/> "))
                    {
                        meta = "prompt";
                        _jniorWebSocket.MetaResponses[meta] = json;
                    }

                }
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, new ExceptionEventArgs(ex));
            }
        }



        public async void Open()
        {
            Console.WriteLine("ConsoleSession.Open: " + _jniorWebSocket.ConsoleOpen);

            if (!_jniorWebSocket.ConsoleOpen && !_jniorWebSocket.ConsoleOpening)
            {
                _jniorWebSocket.ConsoleOpening = true;

                Log?.Invoke(this, new LogEventArgs("Open Console\r\n"));

                var consoleOpen = new ConsoleOpen();
                _jniorWebSocket.Send(consoleOpen);
                if (!await _jniorWebSocket.WaitForMeta("loginprompt"))
                {
                    throw new Exception("Login prompt not received");
                }

                Log?.Invoke(this, new LogEventArgs("Send Username\r\n"));
                var consoleUsername = new ConsoleStdin(_jniorWebSocket.Credentials.UserName + "\r");
                _jniorWebSocket.Send(consoleUsername);
                if (!await _jniorWebSocket.WaitForMeta("passwordprompt"))
                {
                    throw new Exception("Password prompt not received");
                }

                Log?.Invoke(this, new LogEventArgs("Send Password\r\n"));
                var consolePassword = new ConsoleStdin(_jniorWebSocket.Credentials.Password + "\r");
                _jniorWebSocket.Send(consolePassword);
                if (!await _jniorWebSocket.WaitForMeta("prompt"))
                {
                    //throw new Exception("Prompt not received");
                }

                Log?.Invoke(this, new LogEventArgs("Logged in\r\n"));

                _jniorWebSocket.ConsoleOpen = true;
                _jniorWebSocket.ConsoleOpening = false;
            }
        }



        public void Close()
        {
            if (_jniorWebSocket.ConsoleOpen)
            {
                Log?.Invoke(this, new LogEventArgs("Close Console\r\n"));
                _jniorWebSocket.Send(new ConsoleClose());
            }
        }



        public void Send(string v)
        {
            // make sure the session is open
            if (!_jniorWebSocket.ConsoleOpen)
            {
                Open();
            }

            _jniorWebSocket.Send(new ConsoleStdin(v));
        }
    }
}