using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using WebSocketExample.Sorters;
using WebSocketExample.Editors;
using WebSocketExample.Commands;

namespace WebSocketExample.Actions
{
    class RegistryIngestAction : ActionBase
    {
        public static ActionBase CreateAction(string name)
        {
            var json = new JObject();
            json["Action"] = "registry-ingest";
            json["Name"] = name;
            return new RegistryIngestAction(json);
        }



        public RegistryIngestAction(JToken actionJson)
            : base(actionJson)
        {
        }



        [Editor(typeof(IniFileSelectEditor), typeof(System.Drawing.Design.UITypeEditor)), PropertyOrder(10)]
        public string FileName
        {
            get
            {
                var filename = (string)ActionJson["FileName"];
                if (AbsoluteFilePath == null && filename != null)
                {
                    var fileInfo = new FileInfo(Path.Combine("temp", filename));
                    AbsoluteFilePath = fileInfo.FullName;
                    ActionJson["MD5"] = GetMd5FromFile(AbsoluteFilePath);
                    ActionJsonUpdated();
                }
                return filename;
            }
            set
            {
                var fileInfo = new FileInfo(value);
                AbsoluteFilePath = fileInfo.FullName;
                ActionJson["FileName"] = fileInfo.Name;
                ActionJson["MD5"] = GetMd5FromFile(AbsoluteFilePath);
                ActionJsonUpdated();
            }
        }



        public override void Execute(UpdateProjectEngine updateEngine)
        {
            try
            {
                ActionResult = ActionResult.InProgress;

                //// send the file
                var fileInfo = new FileInfo(AbsoluteFilePath);

                var sendFile = new SendFileCommand(JniorWebSocket, fileInfo.FullName, "temp");
                sendFile.Log += JniorWebSocket_Log;
                sendFile.Execute();

                if (sendFile.Completed)
                {
                    var registryIngestCommand = new RegistryIngestCommand(JniorWebSocket, "/temp/" + fileInfo.Name);
                    registryIngestCommand.Log += JniorWebSocket_Log;
                    registryIngestCommand.Complete += JrUpdateCommand_Complete;
                    registryIngestCommand.Execute();
                }
                else
                {
                    throw new Exception("Registry Ingest failed");
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


        private void JrUpdateCommand_Complete(object sender, EventArgs e)
        {
            OnActionProgressChanged();
        }



        public override void GetSteps(ref List<ActionBase> steps)
        {
            steps.Add(this);
        }



        private string GetMd5FromFile(string filename)
        {
            if (!File.Exists(filename))
                return "";
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var md5Bytes = md5.ComputeHash(stream);
                    return BitConverter.ToString(md5Bytes).Replace("-", "");
                }
            }
        }
    }
}
