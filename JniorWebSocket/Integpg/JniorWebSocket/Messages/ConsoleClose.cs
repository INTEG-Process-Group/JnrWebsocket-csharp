namespace Integpg.JniorWebSocket.Messages
{
    public class ConsoleClose : JniorMessage
    {
        public ConsoleClose()
        {
            this["Message"] = "Console Close";
        }
    }
}