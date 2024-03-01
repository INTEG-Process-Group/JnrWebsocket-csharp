using Newtonsoft.Json.Linq;

namespace WebSocketExample.Actions
{
    static class TelnetCommandFactory
    {
        public static ActionBase CreateTelnetCommand(JToken actionJson)
        {
            var telnetCommand = (string)actionJson["Command"];

            if (telnetCommand != null)
            {
                telnetCommand = telnetCommand.TrimStart().ToLower();
                if (telnetCommand.StartsWith("reg ") || telnetCommand.StartsWith("registry ")) return new RegistryTelnetCommandAction(actionJson);
                //else if (telnetCommand.StartsWith("date ")) return new DateTelnetCommandAction(actionJson);
            }

            return null;
        }
    }
}
