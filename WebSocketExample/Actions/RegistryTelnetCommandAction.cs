using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using WebSocketExample.Commands;

namespace WebSocketExample.Actions
{
    class RegistryTelnetCommandAction : ActionBase
    {
        private string _registryTelnetCommand = "";
        private string _registryKey = "";
        private string _registryValue = "";
        private string _remoteValue = "";



        public RegistryTelnetCommandAction(JToken actionJson)
            : base(actionJson)
        {
        }



        public override void Execute(UpdateProjectEngine updateEngine)
        {
            try
            {
                _registryTelnetCommand = (string)ActionJson["Command"];

                GetRegistryKeyFromCommand();
                GetRegistryValueFromCommand();
                GetRemoteValue();

                if (_registryValue.Equals(_remoteValue, StringComparison.CurrentCultureIgnoreCase))
                {
                    SendUpdate(_registryKey + " up to date");
                    ActionResult = ActionResult.NotNeeded;
                }
                else
                {
                    var command = new TelnetCommand(JniorWebSocket, _registryTelnetCommand);
                    command.Log += JniorWebSocket_Log;
                    command.Execute();
                    SendUpdate(command.Response);

                    ActionResult = ActionResult.Success;
                }
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



        private void GetRegistryKeyFromCommand()
        {
            var mypattern = new Regex(@"reg (.*) =");
            _registryKey = mypattern.Match(_registryTelnetCommand).Groups[1].Value;
        }



        private void GetRegistryValueFromCommand()
        {
            var mypattern = new Regex(@"= (.*)");
            _registryValue = mypattern.Match(_registryTelnetCommand).Groups[1].Value;
        }



        private void GetRemoteValue()
        {
            var command = new TelnetCommand(JniorWebSocket, "reg " + _registryKey);
            command.Log += JniorWebSocket_Log;
            command.Execute();
            SendUpdate(command.Response);
            var mypattern = new Regex(@"= (.*)");
            _remoteValue = mypattern.Match(command.Response).Groups[1].Value;
        }
    }
}
