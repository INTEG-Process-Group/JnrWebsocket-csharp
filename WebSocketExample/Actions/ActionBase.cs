using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using WebSocketExample.Sorters;
using System.Threading;
using System.Net;
using Integpg.JniorWebSocket;

namespace WebSocketExample.Actions
{
    [TypeConverter(typeof(PropertySorter))]
    public abstract class ActionBase : IAction
    {
        public event InformationEventHandler UpdateInfo;
        public event EventHandler ActionProgressChanged;
        public event EventHandler ConfigurationUpdated;



        [Browsable(false)]
        public Exception Error { get; protected set; }

        private ActionResult _actionResult;

        [Browsable(false)]
        public ActionResult ActionResult
        {
            get { return _actionResult; }
            protected set
            {
                _actionResult = value;
                ActionProgressChanged?.Invoke(this, EventArgs.Empty);
            }
        }



        [PropertyOrder(1)]
        public string Name
        {
            get { return (string)ActionJson["Name"]; }
            set
            {
                ActionJson["Name"] = value;
                ActionJsonUpdated();
            }
        }



        [PropertyOrder(2), DefaultValue(true)]
        public bool Enabled
        {
            get
            {
                var enabled = (bool)(ActionJson["Enabled"] ?? true);
                return enabled;
            }
            set
            {
                ActionJson["Enabled"] = value;
                ActionJsonUpdated();
            }
        }



        [Browsable(false)]
        public string AbsoluteFilePath
        {
            get { return (string)ActionJson["AbsoluteFilePath"]; }
            protected set
            {
                ActionJson["AbsoluteFilePath"] = value;
                ActionJsonUpdated();
            }
        }



        [Browsable(false)]
        public JToken ActionJson { get; protected set; }



        [Browsable(false)]
        public UpdateProjectEngine UpdateEngine { get; private set; }



        [Browsable(false)]
        public ConnectionProperties ConnectionProperties { get; private set; }



        [Browsable(false)]
        public NetworkCredential Credentials { get; private set; }



        public JniorWebSocket _jniorWebSocket;
        [Browsable(false)]
        public JniorWebSocket JniorWebSocket
        {
            get
            {
                if (_jniorWebSocket == null || !_jniorWebSocket.IsOpened)
                {
                    try
                    {
                        // are we currently connected?
                        if (null == _jniorWebSocket)
                        {
                            Connect(ConnectionProperties.IpAddress, ConnectionProperties.Port, ConnectionProperties.IsSecure);
                        }

                        // wait till we are authenticated
                        while (!_jniorWebSocket.IsAuthenticated)
                        {
                            Thread.Sleep(50);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to establish a telnet connection to the selected unit");
                    }
                }
                return _jniorWebSocket;
            }
            set { _jniorWebSocket = value; }
        }



        protected void ActionJsonUpdated()
        {
            ConfigurationUpdated?.Invoke(this, EventArgs.Empty);
        }



        public ActionBase(JToken actionJson)
        {
            ActionJson = actionJson;
        }



        private void Connect(string ipAddress, int port, bool secure)
        {
            if (!_jniorWebSocket.IsOpened)
            {
                // create the jnior web socket object with the IP Address that we want to connect to
                _jniorWebSocket = new JniorWebSocket(ipAddress);

                // if we are connecting securely we need to specify to accept untrusted certificates
                _jniorWebSocket.IsSecure = secure;
                _jniorWebSocket.AllowUnstrustedCertificate = secure;

                // connect!
                _jniorWebSocket.Connect();
            }
        }



        protected void JniorWebSocket_Log(object sender, LogEventArgs e)
        {
            //Console.WriteLine(e.Message + string.Format("  StackTrace: '{0}'", Environment.StackTrace));
            OnUpdateInfo(new InformationEventArgs(e.Message));
        }



        public void Execute(UpdateProjectEngine updateEngine, JniorWebSocket jniorWebSocket)
        {
            if (Enabled)
            {
                UpdateEngine = updateEngine;
                JniorWebSocket = jniorWebSocket;
                Execute(updateEngine);
            }
        }



        public abstract void Execute(UpdateProjectEngine updateEngine);



        public abstract void GetSteps(ref List<ActionBase> steps);



        public void Reset()
        {
            if (Enabled)
            {
                ActionResult = ActionResult.Waiting;
                SendUpdate("Waiting...");
            }
            else
            {
                ActionResult = ActionResult.NotEnabled;
                SendUpdate("Not Enabled...");
            }
        }



        protected void SendUpdate(string message)
        {
            SendUpdate(message, true);
        }



        protected void SendUpdate(string message, bool log)
        {
            OnUpdateInfo(new InformationEventArgs(message, log));
        }



        protected virtual void OnUpdateInfo(InformationEventArgs e)
        {
            UpdateInfo?.Invoke(this, e);
        }



        protected virtual void OnActionProgressChanged()
        {
            ActionProgressChanged?.Invoke(this, EventArgs.Empty);
        }



        protected virtual void OnConfigurationUpdated()
        {
            ConfigurationUpdated?.Invoke(this, EventArgs.Empty);
        }

    }
}
