namespace Integpg.JniorWebSocket.Messages
{
    class ConsoleStdin : JniorMessage
    {
        public ConsoleStdin(string data) : base("Console Stdin")
        {
            this["Data"] = data;
        }
    }
}