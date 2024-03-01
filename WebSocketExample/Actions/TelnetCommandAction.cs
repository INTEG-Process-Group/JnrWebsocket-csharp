using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using WebSocketExample.Commands;

namespace WebSocketExample.Actions
{
    class TelnetCommandAction : ActionBase
    {
        public TelnetCommandAction(JToken actionJson)
            : base(actionJson)
        {
        }



        public override void Execute(UpdateProjectEngine updateEngine)
        {
            try
            {
                ActionResult = ActionResult.InProgress;

                var actionBase = TelnetCommandFactory.CreateTelnetCommand(ActionJson);

                if (actionBase != null)
                {
                    actionBase.UpdateInfo += actionBase_UpdateInfo;
                    actionBase.Execute(updateEngine, JniorWebSocket);
                    actionBase.UpdateInfo -= actionBase_UpdateInfo;
                }
                else
                {
                    var telnetCommand = (string)ActionJson["Command"];
                    var command = new TelnetCommand(JniorWebSocket, telnetCommand);
                    command.Log += JniorWebSocket_Log;
                    command.Execute();
                    SendUpdate(command.Response);

                    ActionResult = ActionResult.Success;
                }
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
            var actionBase = TelnetCommandFactory.CreateTelnetCommand(ActionJson);
            if (actionBase != null)
                actionBase.GetSteps(ref steps);
            else
                steps.Add(this);
        }



        void actionBase_UpdateInfo(object sender, InformationEventArgs args)
        {
            SendUpdate(args.Message);
        }
    }
}
