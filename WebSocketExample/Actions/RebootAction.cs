using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using WebSocketExample.Commands;

namespace WebSocketExample.Actions
{
    internal class RebootAction : ActionBase
    {
        public static ActionBase CreateAction(string name)
        {
            var json = new JObject();
            json["Action"] = "reboot";
            json["Name"] = name;
            return new RebootAction(json);
        }



        public RebootAction(JToken actionJson)
            : base(actionJson)
        {
        }



        public override void Execute(UpdateProjectEngine updateEngine)
        {
            try
            {
                var rebootCommand = new RebootCommand(JniorWebSocket);
                rebootCommand.Log += JniorWebSocket_Log;
                rebootCommand.Execute();

                ActionResult = ActionResult.Success;
            }
            catch (Exception ex)
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
        }
    }
}
