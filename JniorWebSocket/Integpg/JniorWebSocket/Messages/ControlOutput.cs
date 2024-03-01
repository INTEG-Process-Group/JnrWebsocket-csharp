namespace Integpg.JniorWebSocket.Messages
{
    public class ControlOutput : JniorMessage
    {
        public ControlOutput(string command, int channel)
        {
            this["Message"] = "Control";
            this["Command"] = command;
            this["Channel"] = channel;
        }
    }
}