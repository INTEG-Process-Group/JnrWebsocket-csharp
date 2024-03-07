using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Integpg.JniorWebSocket.Messages
{
    public class PostMessage : JniorMessage
    {
        public PostMessage(int number, JObject content)
        {
            this["Message"] = "Post Message";
            this["Number"] = number;
            this["Content"] = content.ToString(Formatting.None);
        }
    }
}