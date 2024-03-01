namespace com.integpg.websocket.messages
{
    internal class ControlOutput : JniorMessage
    {
        public ControlOutput(string command, int channel)
        {
            this["Message"] = "Control";
            this["Command"] = command;
            this["Channel"] = channel;
        }
    }
}