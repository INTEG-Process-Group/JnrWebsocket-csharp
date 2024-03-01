using System;

namespace Integpg.JniorWebSocket
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; }



        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }
    }
}
