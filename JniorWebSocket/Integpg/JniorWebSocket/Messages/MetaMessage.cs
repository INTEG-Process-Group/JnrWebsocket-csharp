using System;

namespace Integpg.JniorWebSocket.Messages
{
    // extends the JObject Newtonsonft JSON object
    public class MetaMessage : JniorMessage
    {
        private MetaMessage() : base()
        {
        }



        protected MetaMessage(string message) : base(message)
        {
            this["Meta"] = new Random().Next().ToString("X");
        }
    }
}
