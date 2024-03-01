namespace Integpg.JniorWebSocket.Messages
{
    public class ClockSet : JniorMessage
    {
        public ClockSet(long timeSinceJan11970)
        {
            this["Message"] = "Clock Set";
            this["Time"] = timeSinceJan11970;
        }
    }
}