using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using WebSocketExample.Sorters;

namespace WebSocketExample.Actions
{
    class TelnetCommandsAction : ActionBase
    {
        public static ActionBase CreateAction(string name)
        {
            var json = new JObject();
            json["Action"] = "telnet-commands";
            json["Name"] = name;
            return new TelnetCommandsAction(json);
        }



        public TelnetCommandsAction(JToken actionJson)
            : base(actionJson)
        {
        }



        [PropertyOrder(10)]
        public string[] Commands
        {
            get
            {
                var commands = ActionJson["Commands"];
                if (commands != null)
                {
                    return ((JArray)commands).ToObject<string[]>();
                }
                return new string[0];
            }
            set
            {
                ActionJson["Commands"] = new JArray(value);
                ActionJsonUpdated();
            }
        }



        public override void Execute(UpdateProjectEngine updateEngine)
        {
            try
            {
                var commandsResult = ActionResult.InProgress;

                foreach (var command in Commands)
                {
                    var telnetCommandJson = new JObject();
                    telnetCommandJson["Name"] = command;
                    telnetCommandJson["Command"] = command;

                    var telnetCommand = new TelnetCommandAction(telnetCommandJson);
                    //telnetCommand.UpdatePackage = UpdatePackage;
                    telnetCommand.UpdateInfo += telnetCommand_UpdateInfo;
                    telnetCommand.Execute(updateEngine, JniorWebSocket);
                }

                ActionResult = commandsResult;
            }
            catch (System.Exception ex)
            {
                if (!updateEngine.IsCancelled)
                {
                    Error = ex;
                    ActionResult = ActionResult.Failed;
                }
                else
                {
                    ActionResult = ActionResult.Cancelled;
                }
            }
        }



        public override void GetSteps(ref List<ActionBase> steps)
        {
            steps.Add(this);
            //var commandArray = (JArray)ActionJson["Commands"];
            //foreach (var command in commandArray)
            //{
            //    var telnetCommandJson = new JObject();
            //    telnetCommandJson["Name"] = command.Value<string>();
            //    telnetCommandJson["Command"] = command.Value<string>();
            //    var telnetCommand = new TelnetCommandAction(telnetCommandJson);
            //    telnetCommand.GetSteps(ref steps);
            //}
        }



        void telnetCommand_UpdateInfo(object sender, InformationEventArgs args)
        {
            SendUpdate(args.Message);
        }
    }
}
