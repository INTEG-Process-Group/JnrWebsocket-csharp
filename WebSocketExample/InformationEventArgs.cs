using System;

namespace WebSocketExample
{
    public class InformationEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public bool Log { get; private set; }



        public InformationEventArgs(string message)
        {
            Message = message;
            Log = true;
        }



        public InformationEventArgs(string message, bool log)
        {
            Message = message;
            Log = log;
        }
    }
}