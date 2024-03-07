using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Integpg.JniorWebSocket
{
    public class WebSocket : IDisposable
    {
        public static bool Debug { get; private set; }


        public event EventHandler<LogEventArgs> Log;
        public event EventHandler Closed;
        public event EventHandler<ExceptionEventArgs> Error;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler Opened;

        public bool IsSecure { get; private set; }
        public bool AllowUnstrustedCertificate { get; set; }
        public bool Closing { get; private set; }

        private TcpClient _client;
        private string _host;
        private int _port = 80;

        private BinaryReader _in;
        private BinaryWriter _out;

        private byte[] _readBuffer = new byte[4096];
        private byte[] _writeBuffer = new byte[4096];

        private Thread _thread;



        public WebSocket(string uri)
        {
            IsSecure = uri.StartsWith("wss://");

            int hostPos = uri.IndexOf("://") + 3;
            int colonPos = uri.IndexOf(":", 6);
            if (-1 != colonPos)
            {
                _host = uri.Substring(hostPos, colonPos - hostPos);
                _port = int.Parse(uri.Substring(colonPos + 1));
            }
            else
            {
                _host = uri.Substring(hostPos);
                if (IsSecure)
                    _port = 443;
            }
        }



        public WebSocket(TcpClient client)
        {
            InitConnection();
        }



        public void Dispose()
        {
            Close();
        }



        public void Open()
        {
            try
            {
                //_client = new TcpClient(_host, _port);

                _client = new TcpClient();

                // Connect using a timeout (5 seconds)
                var remoteEndPoint = new IPEndPoint(IPAddress.Parse(_host), _port);
                IAsyncResult result = _client.BeginConnect(_host, _port, null, null);

                bool success = result.AsyncWaitHandle.WaitOne(2000, true);

                if (_client.Connected)
                {
                    _client.EndConnect(result);

                    var localEndPoint = (IPEndPoint)_client.Client.LocalEndPoint;
                    Log?.Invoke(this, new LogEventArgs("Connected to " + _host + ":" + _port + " on local port " + localEndPoint.Port + "\r\n"));
                }
                else
                {
                    // NOTE, MUST CLOSE THE SOCKET

                    _client.Close();
                    throw new ApplicationException("Failed to connect server.");
                }

                InitConnection();
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, new ExceptionEventArgs(ex));
            }
        }



        private void InitConnection()
        {
            try
            {
                _in = new BinaryReader(_client.GetStream());
                _out = new BinaryWriter(_client.GetStream());

                /**
                 * i am removing secure for now as i need ot build the support tool with this library.  
                 * kmc 04042018
                 */
                if (IsSecure)
                {
                    Thread.Sleep(200);

                    var ipEndPoint = new IPEndPoint(IPAddress.Parse(_host), _port);
                    SslStream ssl = new SslStream(_client.GetStream(), true, RemoteCertificateValidationCallback);
                    ssl.AuthenticateAsClient(ipEndPoint.Address.ToString(), null, System.Security.Authentication.SslProtocols.Tls12, false);
                    _in = new BinaryReader(ssl);
                    _out = new BinaryWriter(ssl);
                }

                var getRequest = "GET / HTTP/1.1\r\n"
                        + "Upgrade: WebSocket\r\n"
                        + "Connection: Upgrade\r\n"
                        + "Sec-WebSocket-Version: 13\r\n"
                        + "Sec-WebSocket-Key: ZTUxNjYwYTYtOTk1OS00Mw==\r\n"
                        + "Host: " + _host + "\r\n"
                        + "Origin: ws://" + _host + "\r\n\r\n";
                Log?.Invoke(this, new LogEventArgs("GET Upgrade Connection"));
                var getRequestBytes = Encoding.ASCII.GetBytes(getRequest); //{Message:\'\'}".getBytes();
                _out.Write(getRequestBytes);
                _out.Flush();

                var bytesRead = _in.Read(_readBuffer, 0, _readBuffer.Length);
                var upgradeResponse = Encoding.ASCII.GetString(_readBuffer, 0, bytesRead);
                Console.WriteLine(upgradeResponse);
                Log?.Invoke(this, new LogEventArgs("read " + bytesRead + " bytes: " + upgradeResponse.Substring(0, upgradeResponse.IndexOf('\n')) + "\r\n"));

                Opened?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, new ExceptionEventArgs(ex));
            }

            _thread = new Thread(delegate ()
                {
                    try
                    {
                        Closing = false;
                        while (true)
                        {
                            var frame = GetFrame();
                            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(frame));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!Closing)
                        {
                            Error?.Invoke(this, new ExceptionEventArgs(ex));
                            Close(false);
                        }
                    }
                });
            _thread.IsBackground = true;
            _thread.Start();
        }



        public void Close()
        {
            Close(true);
        }



        public void Close(bool graceful)
        {
            if (null != _client)
                _client.Close();

            Closing = graceful;
            //_thread.Interrupt();

            Closed?.Invoke(this, EventArgs.Empty);
        }



        public static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            // accept the remote certificate, because I said so!
            return true;
        }



        public void Send(string message)
        {
            SendFrame(message);
        }



        private string GetFrame()
        {
            lock (_in)
            {
                try
                {
                    byte byte1 = _in.ReadByte();
                    //FileLogger.Info("WebSocket", "byte1 " + byte1);

                    int opCode = (byte1 & 0xf);
                    //FileLogger.Info("WebSocket", "opcode " + opCode);
                    switch (opCode)
                    {
                        case 0:
                            //                System.out.println("continuation frame");
                            break;
                        case 1:
                            //                System.out.println("text frame");
                            break;
                        case 2:
                            //                System.out.println("binary frame");
                            break;
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                            //                System.out.println("non-control frames");
                            break;
                        case 8:
                            //                System.out.println("connection close");
                            break;
                        case 9:
                            //                System.out.println("ping");
                            break;
                        case 0xa:
                            //                System.out.println("pong");
                            break;
                    }

                    byte byte2 = _in.ReadByte();
                    //FileLogger.Info("WebSocket", "byte2 " + byte2);

                    bool mask = (byte2 & 0x80) != 0;
                    //FileLogger.Info("WebSocket", "mask " + mask);

                    ulong len = (ulong)(byte2 & 0x7f);
                    //FileLogger.Info("WebSocket", "len " + len);

                    if (len == 126)
                    {
                        len = _in.ReadUInt16();
                        len = ((len & 0xff00) >> 8) | ((len & 0x00ff) << 8);
                    }
                    else if (len == 127)
                    {
                        len = _in.ReadUInt64();
                        len = (len & 0x00000000000000FF) << 56 | (len & 0x000000000000FF00) << 40 | (len & 0x0000000000FF0000) << 24 | (len & 0x00000000FF000000) << 8 |
                             (len & 0x000000FF00000000) >> 8 | (len & 0x0000FF0000000000) >> 24 | (len & 0x00FF000000000000) >> 40 | (len & 0xFF00000000000000) >> 56;
                    }

                    //byte[] maskingKey = br.ReadBytes(4);

                    try
                    {
                        //FileLogger.Info("WebSocket", "read " + len + " bytes");
                        _readBuffer = _in.ReadBytes((int)len);
                        //FileLogger.Info("WebSocket", len + " bytes read");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    //        System.out.println("mask: " + mask);
                    //        System.out.println("len: " + len);

                    ////for (int i = 0; i < len; i++)
                    ////{
                    ////    int j = i % 4;
                    ////    _readBuffer[i] = (byte)(_readBuffer[i] ^ maskingKey[j]);
                    ////}

                    if (len > (ulong)_readBuffer.Length)
                        _readBuffer = new byte[len];
                    var message = Encoding.ASCII.GetString(_readBuffer, 0, (int)len);

                    if (Debug)
                    {
                        var localEndPoint = _client.Client.LocalEndPoint as IPEndPoint;
                        Console.WriteLine(localEndPoint.Port + " RECV <-- " + message.Replace("\r\n", ""));
                    }

                    return message;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }



        public void SendFrame(string message)
        {
            Log?.Invoke(this, new LogEventArgs("SEND -->: " + message.Replace("\r\n", "")));

            lock (_out)
            {
                try
                {
                    int length = message.Length;
                    if (_writeBuffer.Length < length + 8)
                        _writeBuffer = new byte[length + 8];

                    int pos = 0;
                    _writeBuffer[pos++] = (byte)(0x81);

                    if (length <= 125)
                    {
                        _writeBuffer[pos++] = (byte)length;
                    }
                    else if (length < 0x10000)
                    {
                        _writeBuffer[pos++] = (byte)126;
                        _writeBuffer[pos++] = (byte)((length >> 8) & 0xff);
                        _writeBuffer[pos++] = (byte)(length & 0xff);
                    }
                    _writeBuffer[1] |= 0x80;

                    byte[] _maskingBuffer = new byte[4];
                    for (int j = 0; j < 4; j++)
                    {
                        _maskingBuffer[j] = (byte)(new Random().Next(0, 255));
                    }
                    Array.Copy(_maskingBuffer, 0, _writeBuffer, pos, 4);
                    pos += 4;

                    byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                    for (int i = 0; i < length; i++)
                    {
                        try
                        {
                            _writeBuffer[i + pos] = (byte)(messageBytes[i] ^ _maskingBuffer[i % 4]);
                        }
                        catch (Exception ex)
                        {
                            string exmessage = string.Format("Exception trying to send: {0}\r\nException: {1}\r\nStack: {1}", message, ex.Message, ex.StackTrace);
                            Console.Write(exmessage);
                        }
                    }

                    _out.Write(_writeBuffer, 0, message.Length + pos);
                    _out.Flush();

                    if (Debug)
                    {
                        var localEndPoint = _client.Client.LocalEndPoint as IPEndPoint;
                        Console.WriteLine(localEndPoint.Port + " SENT --> " + message.Replace("\r\n", ""));
                    }
                }
                catch (Exception ex)
                {
                    string exmessage = string.Format("Exception trying to send: {0}\r\nException: {1}\r\nStack: {1}", message, ex.Message, ex.StackTrace);
                    Console.Write(exmessage);

                    Close();
                }
            }
        }

    }
}
