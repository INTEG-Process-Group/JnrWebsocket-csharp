using System;

namespace Integpg.JniorWebSocket
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; }



        public ExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
