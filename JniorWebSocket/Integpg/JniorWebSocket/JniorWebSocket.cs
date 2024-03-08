using Integpg.JniorWebSocket.Messages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Integpg.JniorWebSocket
{
    public class JniorWebSocket : IDisposable
    {
        public event EventHandler<LogEventArgs> Log;
        public event EventHandler<ExceptionEventArgs> Error;
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler Authorized;
        public event EventHandler<UnauthorizedEventArgs> Unauthorized;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public string Host { get; private set; }
        public int Port { get; private set; }
        public string Uri { get; private set; }
        public NetworkCredential Credentials { get; set; }
        private bool _credentialsUsed = false;


        public WebSocket Websocket { get; private set; }

        public Dictionary<string, JObject> MetaResponses = new Dictionary<string, JObject>();

        private ConsoleSession _consoleSession = null;
        public bool ConsoleOpen = false;
        public bool ConsoleOpening = false;
        private string _nonce;



        public JniorWebSocket(string host) : this(host, 0) { }



        public JniorWebSocket(string host, int port)
        {
            Host = host;
            Port = port;

            Credentials = new NetworkCredential("jnior", "jnior");
        }



        public void Dispose()
        {
            Close();
        }



        public JniorWebSocket(System.Net.Sockets.TcpClient client)
        {
            Websocket = new WebSocket(client);
            Websocket.Log += Websocket_Log;
            Websocket.AllowUnstrustedCertificate = AllowUnstrustedCertificate;
            Websocket.Opened += new EventHandler(websocket_Opened);
            Websocket.Error += Error;
            Websocket.Closed += websocket_Closed;
            Websocket.MessageReceived += Websocket_MessageReceived;
        }



        public bool Closing
        {
            get
            {
                return Websocket.Closing;
            }
        }



        public bool IsOpened { get; private set; }



        public bool IsAuthenticated { get; private set; }



        public bool IsSecure { get; set; }



        public bool AllowUnstrustedCertificate { get; set; }



        public ConsoleSession ConsoleSession
        {
            get
            {
                lock (this)
                {
                    if (null == _consoleSession)
                    {
                        while (!IsOpened || !IsAuthenticated)
                        {
                            Thread.Sleep(50);
                        }

                        Log?.Invoke(this, new LogEventArgs("Create new Console Session"));
                        _consoleSession = new ConsoleSession(this);
                        _consoleSession.Log += Log;
                        _consoleSession.Open();
                    }
                    return _consoleSession;
                }
            }
        }



        public async Task<bool> WaitForMeta(string meta)
        {
            var waitTimeoutSeconds = 5;
            var start = DateTime.Now;
            Console.WriteLine("Wait For Meta Response: " + meta);

            DateTime timeoutExpiration = DateTime.Now.AddSeconds(waitTimeoutSeconds);
            while (DateTime.Now <= timeoutExpiration && !MetaResponses.ContainsKey(meta))
            {
                await Task.Delay(25);
            }
            return DateTime.Now <= timeoutExpiration;
        }



        public void Connect()
        {
            Uri = string.Format("{0}://{1}{2}", IsSecure ? "wss" : "ws", Host, (0 != Port) ? ":" + Port : "");
            Log?.Invoke(this, new LogEventArgs(Uri));

            if (null != Websocket)
            {
                Websocket.Log -= Websocket_Log;
                Websocket.Opened -= websocket_Opened;
                Websocket.Error -= Error;
                Websocket.Closed -= websocket_Closed;
                Websocket.MessageReceived -= Websocket_MessageReceived;
            }

            Websocket = new WebSocket(Uri);
            Websocket.Log += Websocket_Log;
            Websocket.AllowUnstrustedCertificate = AllowUnstrustedCertificate;
            Websocket.Opened += websocket_Opened;
            Websocket.Error += Error;
            Websocket.Closed += websocket_Closed;
            Websocket.MessageReceived += Websocket_MessageReceived;

            //Console.WriteLine("JniorWebSocket::Connect" + string.Format("  StackTrace: '{0}'", Environment.StackTrace));
            Log?.Invoke(this, new LogEventArgs("Connect to " + Uri));
            Websocket.Open();

            _credentialsUsed = false;
        }



        public void Reconnect()
        {
            Connect();
        }



        public void Close()
        {
            lock (this)
            {
                if (null != _consoleSession)
                {
                    _consoleSession.Close();
                }
            }

            Websocket.Close();
        }



        public void Send(JniorMessage jniorMessage)
        {
            try
            {
                Websocket.Send(jniorMessage.ToString());
            }
            catch (Exception ex)
            {
                Log?.Invoke(this, new LogEventArgs("\r\n" + ex.ToString() + "\r\n\r\n"));
            }
        }



        public async Task<JObject> Query(JniorMessage jniorMessage)
        {
            try
            {
                // we will send and wait for a response
                if (null == jniorMessage["Meta"] || null == jniorMessage["Meta"]["Hash"])
                {
                    throw new Exception("Must supply a Meta Hash when using the Query method");
                }

                var metaToWaitFor = (string)jniorMessage["Meta"]["Hash"];
                Console.WriteLine("Meta: " + metaToWaitFor);

                Websocket.Send(jniorMessage.ToString());
                Console.WriteLine("Sent: " + jniorMessage.ToString());

                await WaitForMeta(metaToWaitFor);
                JObject response = MetaResponses[metaToWaitFor];

                // the response was obtained from the dictionary.  now remove it
                MetaResponses.Remove(metaToWaitFor);

                return response;
            }
            catch (Exception ex)
            {
                Log?.Invoke(this, new LogEventArgs("\r\n" + ex.StackTrace.ToString() + "\r\n\r\n"));
                throw ex;
            }
        }



        private void Websocket_Log(object sender, LogEventArgs e)
        {
            Log?.Invoke(this, e);
        }



        private void Websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            new Thread(delegate ()
            {
                try
                {
                    if (string.IsNullOrEmpty(e.Message))
                        return;

                    var json = JObject.Parse(e.Message);

                    Log?.Invoke(this, new LogEventArgs("RECV <- " + json.ToString(Newtonsoft.Json.Formatting.None) + "\r\n"));

                    var message = json["Message"].ToString();
                    if ("Error".Equals(message))
                    {
                        HandleErrorMessage(json);

                    }
                    else if ("Monitor".Equals(message))
                    {
                        if (!IsAuthenticated)
                        {
                            IsAuthenticated = true;
                            Authorized?.Invoke(this, EventArgs.Empty);
                        }

                    }
                    else if ("Authenticated".Equals(message))
                    {
                        if (!IsAuthenticated)
                        {
                            IsAuthenticated = true;
                            Authorized?.Invoke(this, EventArgs.Empty);
                        }

                    }

                    //else if ("Console Response".Equals(message))
                    //{
                    //    var status = (string)json["Status"];
                    //    if ("Established".Equals(status)) _consoleOpen = true;
                    //    else if ("Closed".Equals(status)) _consoleOpen = false;
                    //}
                    //else if ("Console Stdout".Equals(message))
                    //{
                    //    var data = (string)json["Data"];
                    //    var meta = null as string;
                    //    if (data.Contains(" login: ")) meta = "loginprompt";
                    //    else if (data.Contains(" password: ")) meta = "passwordprompt";

                    //    if (null != meta && MetaWaitHandles.ContainsKey(meta))
                    //    {
                    //        var waitEvent = MetaWaitHandles[meta];
                    //        if (null != waitEvent)
                    //        {
                    //            Console.WriteLine("signal meta wait " + meta);
                    //            waitEvent.Set();
                    //        }
                    //    }
                    //}

                    if (null != json["Meta"] && null != json["Meta"]["Hash"])
                    {
                        var meta = (string)json["Meta"]["Hash"];
                        Console.WriteLine("Meta found: " + meta);
                        MetaResponses[meta] = json;

                    }

                    // forward on the message received event to anyone who cares about it
                    MessageReceived?.Invoke(this, e);
                }
                catch (Exception ex)
                {
                    Error?.Invoke(this, new ExceptionEventArgs(ex));
                }
            }).Start();
        }



        public void Login(string username, string password)
        {
            Credentials.UserName = username;
            Credentials.Password = password;
            _credentialsUsed = false;

            var login = new Login(username, password, _nonce);
            Send(login);
        }



        private void HandleErrorMessage(JObject json)
        {
            var text = json["Text"].ToString();
            if ("401 Unauthorized".Equals(text))
            {
                _nonce = json["Nonce"].ToString();
                if (null != Credentials && !_credentialsUsed)
                {
                    Login(Credentials.UserName, Credentials.Password);
                    _credentialsUsed = true;
                }
                else
                {
                    Unauthorized?.Invoke(this, new UnauthorizedEventArgs(_nonce));
                }
            }
        }



        private void websocket_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Websocket Opened, nullify previous Console Session");
            _consoleSession = null;
            IsOpened = true;
            IsAuthenticated = false;
            Connected?.Invoke(this, EventArgs.Empty);
            Send(JniorMessage.Empty);
        }



        private void websocket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Websocket Closed, nullify previous Console Session");
            _consoleSession = null;
            IsOpened = false;
            IsAuthenticated = false;
            Disconnected?.Invoke(this, e);
        }

    }
}