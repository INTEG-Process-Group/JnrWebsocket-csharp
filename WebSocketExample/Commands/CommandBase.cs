using Integpg.JniorWebSocket;
using Newtonsoft.Json.Linq;
using System;

namespace WebSocketExample.Commands
{
    public abstract class CommandBase
    {
        public event EventHandler<LogEventArgs> Log;
        public event EventHandler<ExceptionEventArgs> Error;
        public event EventHandler<EventArgs> Complete;

        public Exception Exception { get; private set; }
        public bool Completed { get; private set; }

        protected JniorWebSocket _jniorWebsocket;
        protected bool _disconnected = false;
        protected bool _reconnected = false;



        protected CommandBase(JniorWebSocket jniorWebsocket)
        {
            _jniorWebsocket = jniorWebsocket;
            _jniorWebsocket.Log += _jniorWebsocket_Log;
            _jniorWebsocket.Connected += JnrWebSocket_Connected;
            _jniorWebsocket.Disconnected += JnrWebSocket_Disconnected;
            _jniorWebsocket.Error += _jniorWebsocket_Error;
        }



        public void Execute()
        {
            try
            {
                DoCommand();
            }
            finally
            {
                CommandFinished();
            }

            Unload();

            Completed = true;
            Complete?.Invoke(this, EventArgs.Empty);
        }


        protected abstract void DoCommand();
        protected abstract void CommandFinished();



        private void Unload()
        {
            _jniorWebsocket.Log -= _jniorWebsocket_Log;
            _jniorWebsocket.Connected -= JnrWebSocket_Connected;
            _jniorWebsocket.Disconnected -= JnrWebSocket_Disconnected;
            _jniorWebsocket.Error -= _jniorWebsocket_Error;
        }



        protected void OnLog(string message)
        {
            Log?.Invoke(this, new LogEventArgs(message));
        }



        protected void OnError(Exception ex)
        {
            Exception = ex;
            Error?.Invoke(this, new ExceptionEventArgs(ex));
        }



        private void _jniorWebsocket_Log(object sender, LogEventArgs e)
        {
            //Console.WriteLine(e.Message + string.Format("  StackTrace: '{0}'", Environment.StackTrace));
            OnLog(e.Message);
        }



        private void JnrWebSocket_Connected(object sender, EventArgs e)
        {
            _reconnected = true;
        }



        private void JnrWebSocket_Disconnected(object sender, EventArgs e)
        {
            _disconnected = true;
        }



        private void _jniorWebsocket_Error(object sender, ExceptionEventArgs e)
        {
            OnError(e.Exception);
        }

    }
}
