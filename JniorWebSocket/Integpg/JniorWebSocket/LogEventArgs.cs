using System;

namespace Integpg.JniorWebSocket
{
    public class LogEventArgs : EventArgs
    {
        public string Message { get; set; }



        public LogEventArgs(string message)
        {
            Message = message;
        }
    }
}
