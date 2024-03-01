using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json.Linq;
using WebSocketExample.Commands;
using WebSocketExample.Editors;
using WebSocketExample.Sorters;
using Integpg.JniorWebSocket;

namespace WebSocketExample.Actions
{
    class UploadFileAction : ActionBase
    {
        public static ActionBase CreateAction(string name)
        {
            var json = new JObject();
            json["Action"] = "upload-file";
            json["Name"] = name;
            return new UploadFileAction(json);
        }



        public UploadFileAction(JToken actionJson)
            : base(actionJson)
        {
        }



        [Editor(typeof(FileSelectEditor), typeof(System.Drawing.Design.UITypeEditor)), PropertyOrder(10)]
        public string FileName
        {
            get
            {
                var filename = (string)ActionJson["FileName"];
                if (AbsoluteFilePath == null && filename != null)
                {
                    var fileInfo = new FileInfo(Path.Combine("temp", filename));
                    AbsoluteFilePath = fileInfo.FullName;
                    MD5 = GetMd5FromFile(AbsoluteFilePath);
                    ActionJsonUpdated();
                }
                return filename;
            }
            set
            {
                var fileInfo = new FileInfo(value);
                AbsoluteFilePath = fileInfo.FullName;
                ActionJson["FileName"] = fileInfo.Name;
                MD5 = GetMd5FromFile(AbsoluteFilePath);                
                ActionJsonUpdated();
            }
        }



        [ReadOnly(true)]
        public string MD5
        {
            get
            {
                return (string)ActionJson["MD5"];
            }
            set
            {
                ActionJson["MD5"] = value;
            }
        }



        [PropertyOrder(11)]
        public string RemoteFolder
        {
            get { return (string)ActionJson["RemoteFolder"]; }
            set
            {
                ActionJson["RemoteFolder"] = value;
                ActionJsonUpdated();
            }
        }



        public override void Execute(UpdateProjectEngine updateEngine)
        {
            try
            {
                ActionResult = ActionResult.InProgress;

                // get the file.  try the absolute path first.  if it doesnt exist see if we can 
                var fileInfo = new FileInfo("temp\\" + FileName);
                //if ()

                JniorWebSocket_Log(this, new LogEventArgs("Current Directory: " + Environment.CurrentDirectory));
                JniorWebSocket_Log(this, new LogEventArgs("fileInfo: " + fileInfo));
                JniorWebSocket_Log(this, new LogEventArgs("fileInfo.Exists: " + fileInfo.Exists));

                if (!fileInfo.Exists)
                {
                    JniorWebSocket_Log(this, new LogEventArgs("AbsoluteFilePath: " + AbsoluteFilePath));
                    fileInfo = new FileInfo(AbsoluteFilePath);
                    JniorWebSocket_Log(this, new LogEventArgs("fileInfo: " + fileInfo));
                }

                // send the file
                var sendFile = new SendFileCommand(JniorWebSocket, fileInfo.FullName, RemoteFolder);
                sendFile.Log += JniorWebSocket_Log;
                sendFile.Execute();

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
            using (var md5 = System.Security.Cryptography.MD5.Create())
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
