using Integpg.JniorWebSocket;
using Integpg.Logging;
using System;
using System.Threading;
using System.Windows.Forms;
using WebSocketExample.Actions;

namespace WebSocketExample.Dialogs
{
    public partial class PublishUpdateProjectDialog : Form
    {
        private string _updateProjectFilePath;
        private JniorWebSocket _jniorWebsocket;
        private UpdateProject _updateProject;
        private UpdateProjectEngine _updateProjectEngine;



        public PublishUpdateProjectDialog(string updateProjectFilePath)
        {
            _updateProjectFilePath = updateProjectFilePath;

            InitializeComponent();
        }



        private void UpdateProjectDialog_Shown(object sender, EventArgs e)
        {
            LoadUpdateProject(_updateProjectFilePath);
        }



        // our logging funciton
        private void LogToWindow(string text)
        {
            if (!richTextBox1.IsDisposed)
            {
                // make sure to invoke this on the main window thread
                richTextBox1?.Invoke((MethodInvoker)delegate
                {
                    richTextBox1?.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + "   " + text);
                    richTextBox1?.Select(richTextBox1.TextLength, 1);
                });
            }
        }



        private void LoadUpdateProject(string filename)
        {
            _updateProject = new UpdateProject(filename);
            _updateProject.ProjectOpened += UpdateProject_ProjectOpened;
            _updateProject.Open();

            Text = _updateProject.Name;
            foreach (var actionBase in _updateProject.Steps)
            {
                actionBase.ActionProgressChanged += ActionBase_ActionProgressChanged;
                actionBase.Reset();
            }
        }



        private void UpdateProject_ProjectOpened(object sender, EventArgs e)
        {
            _updateProject = sender as UpdateProject;
            LogToWindow(_updateProject.Name + " opened.\r\n");
        }



        private void ActionBase_ActionProgressChanged(object sender, EventArgs e)
        {
            var actionBase = sender as ActionBase;
            LogToWindow(actionBase.Name + " " + actionBase.ActionResult + Environment.NewLine);
        }



        private void publishProject()
        {
            var connectProperties = GetConnectProperties();
            if (null != connectProperties)
            {
                var updateThd = new Thread(delegate ()
                {
                    var start = DateTime.Now;

                    btnPublishProject.Invoke((MethodInvoker)delegate ()
                    {
                        btnPublishProject.Enabled = false;
                    });

                    // create the jnior web socket object with the IP Address that we want to connect to
                    _jniorWebsocket = new JniorWebSocket(connectProperties.IpAddress, connectProperties.Port);
                    _jniorWebsocket.Log += JniorWebsocket_Log;
                    _jniorWebsocket.Unauthorized += JniorWebsocket_Unauthorized;

                    if (!_jniorWebsocket.IsSecure && 443 == connectProperties.Port)
                    {
                        _jniorWebsocket.IsSecure = true;
                    }

                    // connect!
                    _jniorWebsocket.Connect();


                    //Connect(connectProperties.IpAddress, connectProperties.Port, connectProperties.IsSecure);

                    // wait till we are authenticated
                    while (!_jniorWebsocket.IsAuthenticated)
                        Thread.Sleep(500);

                    _updateProjectEngine = new UpdateProjectEngine(_jniorWebsocket, _updateProject);
                    _updateProjectEngine.ProgressChanged += UpdateEngine_ProgressChanged;
                    _updateProjectEngine.UpdateComplete += UpdateEngine_UpdateComplete;
                    _updateProjectEngine.Execute();

                    _jniorWebsocket.Close();

                    LogToWindow("Update Completed " + _updateProjectEngine.UpdateResult + Environment.NewLine);

                    var resultString = null as string;
                    switch (_updateProjectEngine.UpdateResult)
                    {
                        case ActionResult.Success:
                            resultString = "Update Successful";
                            break;
                        case ActionResult.NotNeeded:
                            resultString = "Update was not needed";
                            break;
                        case ActionResult.Failed:
                            resultString = "Update Failed: " + _updateProjectEngine.Error.Message;
                            break;
                    }

                    var elapsed = DateTime.Now - start;
                    LogToWindow(resultString + " and took " + elapsed + "\r\n");

                    btnPublishProject.Invoke((MethodInvoker)delegate ()
                    {
                        btnPublishProject.Enabled = true;

                        if (DialogResult.Yes == MessageBox.Show(this, "Would you like to save the update log to a file?", "Save update log?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            var saveFileDialog = new SaveFileDialog();
                            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                            saveFileDialog.DefaultExt = "log";
                            saveFileDialog.FileName = _updateProject.Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + "_" + _updateProjectEngine.UpdateResult + ".log";
                            if (DialogResult.OK == saveFileDialog.ShowDialog(this))
                            {
                                richTextBox1.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
                            }
                        }
                    });
                });
                updateThd.IsBackground = true;
                updateThd.Start();
            }
        }

        private void _jniorWebsocket_Log(object sender, LogEventArgs e)
        {
            throw new NotImplementedException();
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



        private void JniorWebsocket_Log(object sender, LogEventArgs e)
        {
            LogToWindow(e.Message);
        }



        private void JniorWebsocket_Unauthorized(object sender, UnauthorizedEventArgs e)
        {
            LogToWindow("Prompt for credentials");

            Invoke((MethodInvoker)delegate ()
            {
                var jniorWebSocket = sender as JniorWebSocket;

                // prompt for user name and password
                var loginDlg = new LoginDialog(jniorWebSocket.Host);
                loginDlg.UserName = jniorWebSocket.Credentials.UserName;
                loginDlg.Password = jniorWebSocket.Credentials.Password;

                if (DialogResult.OK == loginDlg.ShowDialog())
                {
                    jniorWebSocket?.Login(loginDlg.UserName, loginDlg.Password);
                }
                else
                {
                    // user jniorWebSocket cancel to logging in.  abort the connection
                    _jniorWebsocket?.Close();
                }
            });
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



        private void btnPublishProject_Click(object sender, EventArgs e)
        {
            publishProject();
        }

    }
}
