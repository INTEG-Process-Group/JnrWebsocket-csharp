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
                        closeOutput1ToolStripMenuItem.Enabled = _jniorWebsocket.IsOpened;
                    }

                    //openProjectToolStripMenuItem.Enabled = null == _updateProject;
                    publishProjectToolStripMenuItem.Enabled = null != _updateProject;
                    cancelUpdateToolStripMenuItem.Enabled = null != _updateProjectEngine && _updateProjectEngine.InProgress;
                    //closeOutput1ToolStripMenuItem.Enabled = null != _updateProject && (null == _updateProjectEngine || !_updateProjectEngine.InProgress);
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
            Connect("10.0.0.63", false);

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
            Connect(ipAddress, 443, secure);
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
            //_jniorWebsocket.Connected += _jniorWebsocket_Connected;
            //_jniorWebsocket.Disconnected += _jniorWebsocket_Disconnected;
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



        private void LoadUpdateProject(string filename)
        {
            _updateProject = new UpdateProject(filename);
            _updateProject.ProjectOpened += UpdateProject_ProjectOpened;
            _updateProject.Open();
            //_updatesInProgress.Add(jniorUpdate);

            foreach (var actionBase in _updateProject.Steps)
            {
                actionBase.ActionProgressChanged += ActionBase_ActionProgressChanged;
                actionBase.Reset();
            }
        }



        private void UpdateEngine_ProgressChanged(object sender, InformationEventArgs args)
        {
            LogToWindow(args.Message + Environment.NewLine);

            if (args.Log)
            {
                var log = LogDictionary.GetLog("UpdateEngine");
                log.Info(args.Message);
            }
        }



        private void UpdateEngine_UpdateComplete(object sender, EventArgs e)
        {
            var updateProjectEngine = sender as UpdateProjectEngine;

            LogToWindow("Update Completed " + updateProjectEngine.UpdateResult + Environment.NewLine);

            var log = LogDictionary.GetLog("UpdateEngine");
            log.Info("Update Completed: " + updateProjectEngine.UpdateResult);

            if (null != updateProjectEngine.Error)
            {
                LogToWindow(updateProjectEngine.Error + Environment.NewLine);
                log.Error(updateProjectEngine.Error);
            }
        }



        private void ActionBase_ActionProgressChanged(object sender, EventArgs e)
        {
            var actionBase = sender as ActionBase;
            LogToWindow(actionBase.Name + " " + actionBase.ActionResult + Environment.NewLine);
        }



        private void UpdateProject_ProjectOpened(object sender, EventArgs e)
        {
            _updateProject = sender as UpdateProject;
            LogToWindow(_updateProject.Filename + " opened.\r\n");
            UpdateMenu();
        }



        private void publishProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var updateProjectDialog = new PublishUpdateProjectDialog(_updateProject.Filename);
            updateProjectDialog.ShowDialog(this);
        }



        private void cancelUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _updateProjectEngine?.Cancel();
            UpdateMenu();
        }




        private void Form1_Shown(object sender, EventArgs e)
        {
            UpdateMenu();
        }



        private void ProjectOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = Application.StartupPath;
            ofd.Multiselect = false;
            ofd.Filter = "Jnior Update Project (*.jrup)|*.jrup";
            var ofdResult = ofd.ShowDialog();
            if (ofdResult == DialogResult.OK)
            {
                LoadUpdateProject(ofd.FileName);

                var updateProjectDialog = new PublishUpdateProjectDialog(_updateProject.Filename);
                updateProjectDialog.ShowDialog(this);
            }
        }



        private void ProjectEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new EditPropertiesDlg();
            dlg.SetFilename((null != _updateProject) ? _updateProject.Filename : null);
            if (DialogResult.OK == dlg.ShowDialog(this))
            {
                LoadUpdateProject(dlg.Filename);
            }
        }



        private void ProjectCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _updateProject = null;
            UpdateMenu();
        }



        private void Command_Log(object sender, LogEventArgs e)
        {
            LogToWindow(e.Message + Environment.NewLine);
        }



        private void readFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var connectProperties = GetConnectProperties();
            TakeSnapshot(connectProperties);
        }


        private void TakeSnapshot(ConnectionProperties connectProperties)
        {
            if (null != connectProperties)
            {
                var updateThd = new Thread(delegate ()
                {
                    // create the jnior web socket object with the IP Address that we want to connect to
                    using (var jniorWebsocket = new JniorWebSocket(connectProperties.IpAddress, connectProperties.Port))
                    {
                        jniorWebsocket.Log += _jniorWebsocket_Log;
                        jniorWebsocket.Unauthorized += _jniorWebsocket_Unauthorized;

                        // connect!
                        jniorWebsocket.Connect();

                        // wait till we are authenticated
                        while (!jniorWebsocket.IsAuthenticated)
                            Thread.Sleep(50);

                        var snapshotStart = DateTime.Now;

                        LogToWindow("--- Start Snapshot ---\r\n");
                        //LogToWindow("Filter: " + filter);
                        LogToWindow(" v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\r\n");

                        var command = new TelnetCommand(jniorWebsocket, "date");
                        command.Execute();
                        LogToWindow(command.Response + "\r\n\r\n");

                        command = new TelnetCommand(jniorWebsocket, "stats");
                        command.Execute();
                        LogToWindow(command.Response + "\r\n\r\n");

                        command = new TelnetCommand(jniorWebsocket, "ps");
                        command.Execute();
                        LogToWindow(command.Response + "\r\n\r\n");

                        command = new TelnetCommand(jniorWebsocket, "thd");
                        command.Execute();
                        LogToWindow(command.Response + "\r\n\r\n");

                        command = new TelnetCommand(jniorWebsocket, "netstat");
                        command.Execute();
                        LogToWindow(command.Response + "\r\n\r\n");

                        command = new TelnetCommand(jniorWebsocket, "extern");
                        command.Execute();
                        LogToWindow(command.Response + "\r\n\r\n");

                        command = new TelnetCommand(jniorWebsocket, "iolog");
                        command.Execute();
                        LogToWindow(command.Response + "\r\n\r\n");

                        var snapshotFolder = Application.StartupPath + "\\download\\" + snapshotStart.ToString("yyyyMMdd_HHmmss");
                        if (!Directory.Exists(snapshotFolder))
                        {
                            LogToWindow("Create " + snapshotFolder + "\r\n");
                            Directory.CreateDirectory(snapshotFolder);
                        }

                        LogToWindow("Get jniorsys.log.bak\r\n");
                        command = new TelnetCommand(jniorWebsocket, "cat jniorsys.log.bak");
                        command.Execute();
                        File.WriteAllText("download\\" + snapshotStart.ToString("yyyyMMdd_HHmmss") + "\\jniorsys.log.bak", command.Response);

                        LogToWindow("Get jniorsys.log\r\n");
                        command = new TelnetCommand(jniorWebsocket, "cat jniorsys.log");
                        command.Execute();
                        File.WriteAllText("download\\" + snapshotStart.ToString("yyyyMMdd_HHmmss") + "\\jniorsys.log", command.Response);


                        filesToDownload.Clear();
                        var directoriesToTraverse = new List<string>();
                        directoriesToTraverse.Add("/");

                        while (0 != directoriesToTraverse.Count)
                        {
                            var directory = directoriesToTraverse[0];
                            directoriesToTraverse.RemoveAt(0);

                            var fileListCommand = new FileListCommand(jniorWebsocket, directory);
                            fileListCommand.Log += Command_Log;
                            fileListCommand.Execute();

                            foreach (var fileListInfo in fileListCommand.FileListing)
                            {
                                var filename = (string)fileListInfo["Name"];
                                filename = fileListCommand.RemoteFolder + filename;
                                if (!filename.EndsWith("/"))
                                {
                                    if (!filename.EndsWith("jniorsys.log")
                                        && !filename.EndsWith("jniorsys.log.bak"))
                                    {
                                        fileListInfo["Name"] = filename;
                                        filesToDownload.Add(fileListInfo);
                                    }
                                }
                                else
                                {
                                    directoriesToTraverse.Add(filename);
                                }
                            }
                        }

                        LogToWindow(string.Format("Scheduled To Load {0} files\r\n", filesToDownload.Count));

                        //var downloadedAllFiles = new ManualResetEvent(false);
                        //var downloadLock = new object();
                        //Int32 downloadsInProgress = 0;
                        //var downloadedCount = 0;

                        for (var i = 0; i < filesToDownload.Count; i++)
                        {
                            int asyncDownloadCount = 2;
                            lock (downloadLock)
                            {
                                if (asyncDownloadCount <= downloadsInProgress)
                                {
                                    //LogToWindow(downloadsInProgress + "   too many downloads in progress\r\n");
                                    Monitor.Wait(downloadLock);
                                }
                                downloadsInProgress++;
                            }


                            //new Thread(delegate (object threadContext)
                            //{
                            var index = i;
                            //(int)threadContext;
                            var fileInfo = filesToDownload[index];
                            var filename = (string)fileInfo["Name"];

                            //lock (downloadLock)
                            //{
                            //    while (0 < downloadsInProgress)
                            //    {
                            //        LogToWindow(downloadsInProgress + "   Wait to download " + filename + "\r\n");
                            //        Monitor.Wait(downloadLock);
                            //    }

                            var fileSize = (int)fileInfo["Size"];
                            LogToWindow("Downloading " + filename + " (" + fileSize + ")   " + (index + 1) + " / " + filesToDownload.Count + "\r\n");
                            //downloadsInProgress++;
                            //}

                            var readFileCommand = new ReadFileCommand(jniorWebsocket, filename, "download\\" + snapshotStart.ToString("yyyyMMdd_HHmmss"));
                            readFileCommand.Log += Command_Log;
                            readFileCommand.Error += ReadFileCommand_Error;
                            readFileCommand.Done += ReadFileCommand_Done;
                            readFileCommand.FileInfoJson = fileInfo as JObject;
                            readFileCommand.Execute();

                            //lock (downloadLock)
                            //{
                            //    downloadedCount++;
                            //    downloadsInProgress--;
                            //    LogToWindow(downloadsInProgress + "   Downloaded " + filename + "   " + (downloadedCount + 1) + " / " + filesToDownload.Count + "\r\n");
                            //    Monitor.Pulse(downloadLock);
                            //}

                            //if (filesToDownload.Count <= downloadedCount)
                            //{
                            //    downloadedAllFiles.Set();
                            //}
                            //}).Start(i);
                        }

                        //downloadedAllFiles.WaitOne();

                        command = new TelnetCommand(jniorWebsocket, "rm jniorio.log");
                        command.Execute();
                        LogToWindow(command.Response + "\r\n\r\n");

                        var elapsed = DateTime.Now - snapshotStart;
                        LogToWindow("snapshot took " + elapsed + "\r\n");
                        File.WriteAllText(snapshotFolder + "\\SNAPSHOT.LOG", textBox1.Text);

                        lock (downloadLock)
                        {
                            if (0 < downloadsInProgress)
                            {
                                LogToWindow(downloadsInProgress + " downloads still in progress\r\n");
                                Monitor.Wait(downloadLock);
                            }
                            downloadsInProgress++;
                        }


                        var snapshotFilename = "download\\" + snapshotStart.ToString("yyyyMMdd_HHmmss") + ".zip";
                        using (var fsOut = File.Create(snapshotFilename))
                        using (var zipStream = new ZipOutputStream(fsOut))
                        {

                            // This setting will strip the leading part of the folder path in the entries, to
                            // make the entries relative to the starting folder.
                            // To include the full path for each entry up to the drive root, assign folderOffset = 0.
                            var folderOffset = snapshotFolder.Length + (snapshotFolder.EndsWith("\\") ? 0 : 1);

                            CompressFolder(snapshotFolder, zipStream, folderOffset);
                        }

                        // if we successfully created our zip the blow away the folder
                        if (File.Exists(snapshotFilename))
                        {
                            Directory.Delete(snapshotFolder, true);
                        }

                    }
                });
                updateThd.IsBackground = true;
                updateThd.Start();

                UpdateMenu();
            }
        }


        private JArray filesToDownload = new JArray();
        private object downloadLock = new object();
        private Int32 downloadsInProgress = 0;

        private void ReadFileCommand_Done(object sender, EventArgs e)
        {
            var ReadFileCommand = (ReadFileCommand)sender;

            lock (downloadLock)
            {
                //downloadedCount++;
                downloadsInProgress--;
                LogToWindow("   Downloaded " + ReadFileCommand.RemoteFile + "\r\n");
                Monitor.Pulse(downloadLock);
            }
        }



        private void ReadFileCommand_Error(object sender, ExceptionEventArgs e)
        {
            var readFileCommand = (ReadFileCommand)sender;
            LogToWindow(e.Exception + "\r\n");
            filesToDownload.Add(readFileCommand.FileInfoJson);
            LogToWindow("Rescheduled " + readFileCommand.RemoteFile + "\r\n");
        }



        // Recurses down the folder structure
        //
        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {

            var files = Directory.GetFiles(path);

            foreach (var filename in files)
            {

                var fi = new FileInfo(filename);

                var entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                var newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                var buffer = new byte[4096];
                using (var streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            var folders = Directory.GetDirectories(path);
            foreach (var folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }



        private void iPLookupToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var thd = new Thread(delegate ()
            {
                var lines = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\ip-addresses.txt");

                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var parts = line.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var ip = parts[0].Trim();

                    var updateThd = new Thread(delegate ()
                    {
                        try
                        {
                            System.Net.IPHostEntry iphe = System.Net.Dns.GetHostByAddress(ip);
                            LogToWindow(string.Format(", {0},   {1},   {2}\r\n", ip, parts[1], iphe.HostName));
                        }
                        catch (Exception ex)
                        {
                            LogToWindow(ip + " not found\r\n");
                        }
                    });
                    updateThd.IsBackground = true;
                    updateThd.Start();
                }
            });
            thd.Start();
        }



        private void repeatedSnapshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var thd = new Thread(delegate ()
            {
                for (var i = 0; i < 24; i++)
                {
                    var connectionProperties = new ConnectionProperties();
                    connectionProperties.IpAddress = "10.0.0.75";

                    TakeSnapshot(connectionProperties);

                    Thread.Sleep(3600000);
                }
            });
            thd.Start();
        }



        private void openConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null != _jniorWebsocket)
            {
                var consoleOpen = new ConsoleOpen();
                _jniorWebsocket.Send(consoleOpen);
            }
        }



        private void testFlashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var directoryPath = @"C:\Users\kcloutier\Documents\Sandbox\Embedded Applications\INTEG Applications\DmxPort\web";
            var files = new DirectoryInfo(directoryPath).GetFiles("*.*", SearchOption.AllDirectories);

            var thd = new Thread(delegate ()
            {
                var failed = false;

                while (!failed)
                {
                    _log = LogDictionary.GetLog("JrFlashTest-" + DateTime.Now.ToString("HHmm"));

                    foreach (var fileInfo in files)
                    {
                        LogToWindow(fileInfo.FullName + "\r\n");
                        _log.Info(fileInfo.FullName);
                    }


                    var connectionProperties = new ConnectionProperties();
                    connectionProperties.IpAddress = "10.0.0.88";
                    var jniorWebsocket = new JniorWebSocket(connectionProperties.IpAddress, connectionProperties.Port);
                    try
                    {

                        jniorWebsocket.Credentials = new System.Net.NetworkCredential("jnior", "jnior");
                        jniorWebsocket.Connect();

                        // wait till we are authenticated
                        while (!jniorWebsocket.IsAuthenticated)
                        {
                            Thread.Sleep(50);
                        }

                        foreach (var fileInfo in files)
                        {
                            LogToWindow("sending " + fileInfo.FullName + " to " + connectionProperties.IpAddress + "\r\n");
                            _log.Info("sending " + fileInfo.FullName + " to " + connectionProperties.IpAddress);

                            var relativePath = fileInfo.Directory.FullName.Replace(directoryPath, "");
                            relativePath = relativePath.Replace("\\", "/");
                            var sendFile = new SendFileCommand(jniorWebsocket, fileInfo.FullName, "flash/test/www/dmx/" + relativePath);
                            sendFile.Log += JniorWebSocket_Log;
                            sendFile.Execute();

                            if (sendFile.Completed && null == sendFile.Exception)
                            {
                                LogToWindow(fileInfo.FullName + " upload has completed\r\n");
                                _log.Info(fileInfo.FullName + " upload has completed");

                                var md5 = GetMd5FromFile(fileInfo.FullName);
                                md5 = md5.ToLower();

                                //try
                                //{
                                //    // Configure the audio output and speak 
                                //    synth.SetOutputToDefaultAudioDevice();
                                //    synth.Speak(fileInfo.Name + " upload has completed");
                                //}
                                //catch (Exception) { }

                                var remoteFolder = sendFile.RemoteFolder.Replace("/", "\\");
                                var remoteFile = Path.Combine(remoteFolder, fileInfo.Name);
                                remoteFile = remoteFile.Replace("\\", "/");
                                var manifestCommand = new TelnetCommand(jniorWebsocket, "manifest " + remoteFile);
                                manifestCommand.Log += JniorWebSocket_Log;
                                manifestCommand.Execute();
                                LogToWindow(manifestCommand.Response + "\r\n");
                                _log.Info(manifestCommand.Response);

                                var match = manifestCommand.Response.ToLower().Contains(md5);
                                LogToWindow("Match: " + match + "\r\n");
                                _log.Info("Match: " + match);

                                if (!match)
                                {
                                    _log.Info("Match Failed.  Expected MD5: " + md5);
                                    failed = true;
                                    break;
                                }
                            }
                            else
                            {
                                LogToWindow(fileInfo.FullName + " upload has failed\r\n");
                                LogToWindow(fileInfo.FullName + ": " + sendFile.Exception.ToString() + "\r\n");
                                _log.Info(fileInfo.FullName + " upload has failed\r\n");
                                _log.Info(fileInfo.FullName + ": " + sendFile.Exception.ToString());
                            }

                            Thread.Sleep(1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogToWindow(ex.ToString());
                        _log.Info(ex.ToString());
                    }
                    finally
                    {
                        var jrflashDumpCommand = new TelnetCommand(jniorWebsocket, "jrflash -d");
                        jrflashDumpCommand.Log += JniorWebSocket_Log;
                        jrflashDumpCommand.Execute();
                        LogToWindow(jrflashDumpCommand.Response + "\r\n");
                        _log.Info(jrflashDumpCommand.Response);

                        //var jrflashReclaimCommand = new TelnetCommand(jniorWebsocket, "jrflash -r");
                        //jrflashReclaimCommand.Log += JniorWebSocket_Log;
                        //jrflashReclaimCommand.Execute();
                        //LogToWindow(jrflashReclaimCommand.Response + "\r\n");
                        //log.Info(jrflashReclaimCommand.Response );

                        jniorWebsocket.Close();
                    }

                    Thread.Sleep(300000);
                }

                LogToWindow("Uh Oh\r\n");
            });
            thd.Start();
        }



        private string GetMd5FromFile(string filename)
        {
            if (!File.Exists(filename))
                return "";
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var md5Bytes = md5.ComputeHash(stream);
                    return BitConverter.ToString(md5Bytes).Replace("-", "");
                }
            }
        }
    }
}
