using Newtonsoft.Json.Linq;

namespace com.integpg.websocket.messages
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
    }
}
