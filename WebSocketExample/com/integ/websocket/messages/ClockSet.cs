namespace com.integpg.websocket.messages
{
    internal class ClockSet : JniorMessage
    {
        public ClockSet(long timeSinceJan11970)
        {
            this["Message"] = "Clock Set";
            this["Time"] = timeSinceJan11970;
        }
    }
}