namespace Integpg.JniorWebSocket.Messages
{
    public class ClockRead : JniorMessage
    {
        public ClockRead()
        {
            this["Message"] = "Clock Read";
        }
    }
}