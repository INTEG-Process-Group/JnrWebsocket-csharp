using Newtonsoft.Json.Linq;

namespace WebSocketExample.Actions
{
    static class ActionBaseFactory
    {
        public static ActionBase CreateAction(JToken actionJson)
        {
            var actionType = (string)actionJson["Action"];

            if (actionType.Equals("install-os"))
                return new InstallOsAction(actionJson);
            if (actionType.Equals("upload-file"))
                return new UploadFileAction(actionJson);
            if (actionType.Equals("telnet-commands"))
                return new TelnetCommandsAction(actionJson);
            if (actionType.Equals("registry-ingest"))
                return new RegistryIngestAction(actionJson);
            if (actionType.Equals("reboot"))
                return new RebootAction(actionJson);
            throw new System.Exception("Unknown type referenced");
        }
    }
}
