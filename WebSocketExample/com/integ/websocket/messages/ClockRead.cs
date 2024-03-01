namespace com.integpg.websocket.messages
{
    internal class ClockRead : JniorMessage
    {
        public ClockRead()
        {
            this["Message"] = "Clock Read";
        }
    }
}