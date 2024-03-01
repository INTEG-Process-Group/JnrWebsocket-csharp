namespace com.integpg.websocket
{
    public class LogEventArgs
    {
        public string Message { get; set; }



        public LogEventArgs(string message)
        {
            Message = message;
        }
    }
}
