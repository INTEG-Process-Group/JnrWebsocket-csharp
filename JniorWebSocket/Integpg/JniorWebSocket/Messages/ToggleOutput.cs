namespace Integpg.JniorWebSocket.Messages
{
    public class ToggleOutput : ControlOutput
    {
        public ToggleOutput(int channel) : base("Toggle", channel) { }
    }
}
