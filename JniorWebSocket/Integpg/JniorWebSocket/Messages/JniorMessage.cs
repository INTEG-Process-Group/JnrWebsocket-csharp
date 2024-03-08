using Newtonsoft.Json.Linq;
using System;

namespace Integpg.JniorWebSocket.Messages
{
    // extends the JObject Newtonsonft JSON object
    public class JniorMessage : JObject
    {
        public static JniorMessage Empty;



        // creates an empty object that we use when the connection is made.  when the jnior 
        // receives this it will either send us a monitor packet or an unauthorized message
        static JniorMessage()
        {
            Empty = new JniorMessage();
            Empty["Message"] = "";
        }



        protected JniorMessage()
        {
        }



        protected JniorMessage(string message) : this()
        {
            this["Message"] = message;
        }

    }
}
