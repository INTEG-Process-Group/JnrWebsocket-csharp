using Newtonsoft.Json.Linq;
using System;

namespace Integpg.JniorWebSocket.Messages
{
    // extends the JObject Newtonsonft JSON object
    public class MetaMessage : JObject
    {
        public MetaMessage(string prefix = "") : base()
        {
            if (null != prefix) prefix += "-";
            this["Hash"] = $"{prefix}{new Random().Next().ToString("X")}";
        }
    }
}
