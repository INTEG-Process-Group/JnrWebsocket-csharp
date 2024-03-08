using Newtonsoft.Json.Linq;

namespace Integpg.JniorWebSocket.Messages
{
    public class RegistryRead : JniorMessage
    {
        public RegistryRead(string registryKey) : this(new string[] { registryKey })
        {
        }



        public RegistryRead(string[] registryKeys)
        {
            this["Message"] = "Registry Read";
            this["Meta"] = new MetaMessage("reg.read");
            this["Keys"] = new JArray(registryKeys);
        }

    }
}