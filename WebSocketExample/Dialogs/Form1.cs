using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Integpg.JniorWebSocket;
using Integpg.JniorWebSocket.Messages;
using Integpg.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Reflection;
using System.Security.Cryptography;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WebSocketExample.Actions;
using WebSocketExample.Commands;
using WebSocketExample.Dialogs;

namespace WebSocketExample
{
    public partial class Form1 : Form
    {
        private NLog.Logger _log = LogDictionary.GetLog("");

        private JniorWebSocket _jniorWebsocket;
        private bool _reconnected = false;

        private UpdateProject _updateProject;
        private UpdateProjectEngine _updateProjectEngine;

        private static Dictionary<string, FilePushLocation> IpAddressesByFile = new Dictionary<string, FilePushLocation>();
        private static List<string> FileTransfersInProgress = new List<string>();

        // Initialize a new instance of the SpeechSynthesizer.
        private SpeechSynthesizer synth = new SpeechSynthesizer();



        class FilePushLocation
        {
            public string IpAddress { get; private set; }
            public string RemoteFolder { get; private set; }
            public string RemoteName { get; private set; }



            public FilePushLocation(string ipAddress, string remoteFolder)
            {
                IpAddress = ipAddress;
                RemoteFolder = remoteFolder;
            }



            public FilePushLocation(string ipAddress, string remoteFolder, string remoteName)
            {
                IpAddress = ipAddress;
                RemoteFolder = remoteFolder;
                RemoteName = remoteName;
            }
        }



        public Form1()
        {
            InitializeComponent();

            //IpAddressesByFile.Add(@"ProjectorWatchdog\ProjectorWatchdog.jar", new FilePushLocation("10.0.0.75", "flash"));

            //IpAddressesByFile.Add(@"Cinema-JANOS\Cinema.jar", new FilePushLocation("10.0.0.241", "flash"));

            //IpAddressesByFile.Add("DMXPort\\web", new FilePushLocation("10.0.0.241", "flash/public/dmx"));
            //IpAddressesByFile.Add("DMXPort\\dmx.jar", new FilePushLocation("10.0.0.241", "flash", "dmx.jar"));

            //IpAddressesByFile.Add(@"LineSidePrinting\lsp.jar", new FilePushLocation("10.0.0.252", "flash"));
            //IpAddressesByFile.Add(@"LineSidePrinting\web2.0\linesideprinting\index.html", new FilePushLocation("10.0.0.252", "flash/public/linesideprinting"));
            //IpAddressesByFile.Add(@"LineSidePrinting\web2.0\linesideprinting\line-side-printing.js", new FilePushLocation("10.0.0.252", "flash/public/linesideprinting"));

            //IpAddressesByFile.Add("INTEG Applications\\Tasker\\web", new FilePushLocation("10.0.0.73", "flash/public/tasker"));
            //IpAddressesByFile.Add("INTEG Applications\\Tasker\\Tasker.jar", new FilePushLocation("10.0.0.73", "flash"));

            //IpAddressesByFile.Add("INTEG Applications\\Grapher", new FilePushLocation("10.0.0.73", "flash/www/grapher"));
            //IpAddressesByFile.Add("INTEG Applications\\snmp\\snmp.jar", new FilePushLocation("10.0.0.73", "flash"));


            //IpAddressesByFile.Add("INTEG Applications\\ModbusMaster\\web", new FilePushLocation("10.0.0.74", "flash/www/modbusmaster"));
            //IpAddressesByFile.Add("INTEG Applications\\ModbusMaster\\ModbusMaster.jar", new FilePushLocation("10.0.0.74", "flash"));

            //IpAddressesByFile.Add("cfAnalogSlave\\ModbusAnalogSlave.jar", new FilePushLocation("10.0.0.245", "flash"));

            //var fsw = new FileSystemWatcher(@"C:\Users\kcloutier\Documents\Sandbox");
            //fsw.IncludeSubdirectories = true;
            //fsw.Changed += Fsw_Changed;
            //fsw.EnableRaisingEvents = true;
        }



        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                var fileInfo = new FileInfo(e.FullPath);
                var filePushLocation = null as FilePushLocation;

                if (FileAttributes.Directory == (FileAttributes.Directory & new FileInfo(e.FullPath).Attributes))
                {
                    return;
                }

                foreach (var filename in IpAddressesByFile.Keys)
                {
                    if (e.FullPath.ToLower().Contains(filename.ToLower()))
                    {
                        filePushLocation = IpAddressesByFile[filename];
                        break;
                    }
                }
                if (null == filePushLocation)
                {
                    return;
                }


                //if (!IpAddressesByFile.ContainsKey(fileInfo.Name.ToLower()))
                //{
                //    return;
                //}

                lock (FileTransfersInProgress)
                {
                    if (!FileTransfersInProgress.Contains(e.FullPath))
                    {
                        FileTransfersInProgress.Add(e.FullPath);

                        new Thread(delegate ()
                        {
                            Thread.Sleep(1000);

                            var jniorWebsocket = new JniorWebSocket(filePushLocation.IpAddress);
                            try
                            {
                                LogToWindow("sending " + e.FullPath + " to " + filePushLocation.IpAddress + "\r\n");

                                jniorWebsocket.Credentials = new System.Net.NetworkCredential("jnior", "jnior");
                                jniorWebsocket.Connect();

                                // wait till we are authenticated
                                while (!jniorWebsocket.IsAuthenticated)
                                {
                                    Thread.Sleep(50);
                                }

                                var sendFile = new SendFileCommand(jniorWebsocket, e.FullPath, filePushLocation.RemoteFolder, filePushLocation.RemoteName);
                                sendFile.Log += JniorWebSocket_Log;
                                sendFile.Execute();

                                if (sendFile.Completed && null == sendFile.Exception)
                                {
                                    LogToWindow(e.FullPath + " upload has completed\r\n");

                                    try
                                    {
                                        // Configure the audio output and speak 
                                        synth.SetOutputToDefaultAudioDevice();
                                        synth.Speak(fileInfo.Name + " upload has completed");
                                    }
                                    catch (Exception) { }
                                }
                                else
                                {
                                    LogToWindow(e.FullPath + " upload has failed\r\n");
                                    LogToWindow(e.FullPath + ": " + sendFile.Exception.ToString() + "\r\n");
                                }
                            }
                            finally
                            {
                                jniorWebsocket.Close();

                                FileTransfersInProgress.Remove(e.FullPath);
                            }
                        }).Start();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }



        private void JniorWebSocket_Log(object sender, LogEventArgs e)
        {
            LogToWindow(e.Message + "\r\n");
            _log?.Info(e.Message);
        }



        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        // Callback from the jnior web socket object.  here we will log the message that was 
        // received straigh to the log window
        private void _jniorWebsocket_Log(object sender, LogEventArgs args)
        {
            LogToWindow(args.Message + "\r\n");
        }



        // connected callback from the jnior web socket object
        private void _jniorWebsocket_Connected(object sender, EventArgs e)
        {
            LogToWindow("Connected\r\n");
            UpdateMenu();
        }



        // disconnected callback from the jnior web socket object
        private void _jniorWebsocket_Disconnected(object sender, EventArgs e)
        {
            // try to connect in a different thread
            new Thread(delegate ()
            {
                LogToWindow("Disconnected\r\n");
                UpdateMenu();

                if (!_jniorWebsocket.Closing)
                {
                    // try to reconnect
                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(10000);
                        _jniorWebsocket.Reconnect();
                        //Connect(_jniorWebsocket.Host, false);
                        if (_jniorWebsocket.IsOpened)
                        {
                            _reconnected = true;
                            break;
                        }
                    }
                }
            }).Start();
        }



        private void _jniorWebsocket_Error(object sender, ExceptionEventArgs e)
        {
            LogToWindow(e.Exception.ToString() + "\r\n");
        }



        private void _jniorWebsocket_Unauthorized(object sender, UnauthorizedEventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                var jniorWebSocket = sender as JniorWebSocket;

                if (_reconnected)
                {
                    jniorWebSocket?.Login(_jniorWebsocket.Credentials.UserName, _jniorWebsocket.Credentials.Password);
                }
                else
                {
                    // prompt for user name and password
                    var loginDlg = new LoginDialog(jniorWebSocket.Host);
                    loginDlg.UserName = jniorWebSocket.Credentials.UserName;
                    loginDlg.Password = jniorWebSocket.Credentials.Password;

                    if (DialogResult.OK == loginDlg.ShowDialog())
                    {
                        var username = loginDlg.UserName;
                        var password = loginDlg.Password;

                        jniorWebSocket?.Login(username, password);
                    }
                    else
                    {
                        // user jniorWebSocket cancel to logging in.  abort the connection
                        _jniorWebsocket?.Close();
                    }
                }
            });
        }



        private void UpdateMenu()
        {
            if (!IsDisposed)
            {
                Invoke((MethodInvoker)delegate
                {
                    if (null != _jniorWebsocket)
                    {
                        connectToolStripMenuItem.Enabled = !_jniorWebsocket.IsOpened;
                        secureConnectToolStripMenuItem1.Enabled = !_jniorWebsocket.IsOpened;
                        disconnectToolStripMenuItem.Enabled = _jniorWebsocket.IsOpened;

                        fileSystemToolStripMenuItem.Enabled = _jniorWebsocket.IsOpened;

                        closeOutput1ToolStripMenuItem.Enabled = _jniorWebsocket.IsOpened;
                    }
                });
            }
        }



        // our logging funciton
        private void LogToWindow(string text)
        {
            if (!IsDisposed)
            {
                //Console.WriteLine(text + string.Format("   StackTrace: '{0}'", Environment.StackTrace));

                // make sure to invoke this on the main window thread
                Invoke((MethodInvoker)delegate
                {
                    textBox1?.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + "   " + text);
                });
            }
        }



        // toggle menu item click handler
        private void toggleOutput1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // send a new toggle output object for channel 1
            _jniorWebsocket.Send(new ToggleOutput(1));
        }



        // connect menu item click handler
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Connect("10.0.0.63", false);

            var connectProperties = GetConnectProperties();
            if (null != connectProperties)
            {
                Connect(connectProperties.IpAddress, connectProperties.Port, connectProperties.IsSecure);
            }
        }



        private ConnectionProperties GetConnectProperties()
        {
            var connectProperties = new ConnectionProperties();
            if (null != Properties.Settings.Default.LastIpAddress)
                connectProperties.IpAddress = Properties.Settings.Default.LastIpAddress;
            if (null != Properties.Settings.Default.LastIpAddress)
                connectProperties.Port = Properties.Settings.Default.LastPort;
            if (null != Properties.Settings.Default.LastIpAddress)
                connectProperties.IsSecure = Properties.Settings.Default.LastIsSecure;

            var connectDialog = new ConnectDialog();
            connectDialog.ConnectProperties = connectProperties;
            if (DialogResult.OK == connectDialog.ShowDialog(this))
            {
                connectProperties = connectDialog.ConnectProperties;

                if (null != connectProperties)
                {
                    Properties.Settings.Default.LastIpAddress = connectProperties.IpAddress;
                    Properties.Settings.Default.LastPort = connectProperties.Port;
                    Properties.Settings.Default.LastIsSecure = connectProperties.IsSecure;
                    Properties.Settings.Default.Save();
                }

                return connectProperties;
            }
            return null;
        }



        private void secureConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Connect("10.0.0.63", true);
        }



        private void Connect(string ipAddress, bool secure)
        {
            Connect(ipAddress, secure ? 443 : 80, secure);
        }



        private void Connect(string ipAddress, int port, bool secure)
        {
            // create the jnior web socket object with the IP Address that we want to connect to
            _jniorWebsocket = new JniorWebSocket(ipAddress, port);

            // if we are connecting securely we need to specify to accept untrusted certificates
            _jniorWebsocket.IsSecure = secure;
            _jniorWebsocket.AllowUnstrustedCertificate = secure;

            // wireup some callbacks
            _jniorWebsocket.Log += _jniorWebsocket_Log;
            _jniorWebsocket.Error += _jniorWebsocket_Error;
            _jniorWebsocket.Connected += _jniorWebsocket_Connected;
            _jniorWebsocket.Disconnected += _jniorWebsocket_Disconnected;
            _jniorWebsocket.Unauthorized += _jniorWebsocket_Unauthorized;

            // connect!
            _jniorWebsocket.Connect();
        }






        private void readClockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var readClock = new ClockRead();
            _jniorWebsocket.Send(readClock);
        }



        private void setClockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetClock();
        }



        private void SetClock()
        {
            // get the time since epoch JAN 1 1970
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timeSinceJan11970 = DateTime.UtcNow - epoch;

            // set the clock via websocket
            var setClock = new ClockSet((long)timeSinceJan11970.TotalMilliseconds);
            _jniorWebsocket.Send(setClock);
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _jniorWebsocket?.Close();
        }



        private void rebootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _jniorWebsocket.ConsoleSession.Open();
            _jniorWebsocket.ConsoleSession.Send("reboot -f\r");
        }



        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _jniorWebsocket?.Login("jnior", "jnior");
        }



        private void Form1_Shown(object sender, EventArgs e)
        {
            UpdateMenu();
        }


        
        private void openConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null != _jniorWebsocket)
            {
                _jniorWebsocket.ConsoleSession.Open();
            }
        }



        private async void getSerialNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistryRead registryRead = new RegistryRead("$serialnumber");
            JObject registryReponse = await _jniorWebsocket.Query(registryRead);
            LogToWindow(registryReponse.ToString());
        }



        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _jniorWebsocket.Close();
        }



        private async void getRootListingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
            // ask the jnior websocket connection for a file listing for the root folder.  here we will use the 
            // query method so that the program blocks and waits for a response.
            var rootListing = await _jniorWebsocket.Query(new FileList("/"));
            if (null != rootListing)
            {
                LogToWindow(rootListing.ToString());
            }
        }
    }
}
