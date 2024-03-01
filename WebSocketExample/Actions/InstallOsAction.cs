using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using WebSocketExample.Sorters;
using WebSocketExample.Editors;
using WebSocketExample.Commands;

namespace WebSocketExample.Actions
{
    class InstallOsAction : ActionBase
    {
        public static ActionBase CreateAction(string name)
        {
            var json = new JObject();
            json["Action"] = "install-os";
            json["Name"] = name;
            return new InstallOsAction(json);
        }



        public InstallOsAction(JToken actionJson)
            : base(actionJson)
        {
        }



        [Editor(typeof(UdpFileSelectEditor), typeof(System.Drawing.Design.UITypeEditor)), PropertyOrder(10)]
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
                if (!JanosUpd.IsJanosUpd(fileInfo))
                    throw new Exception("Not a valid JANOS UPD");
                Version = JanosUpd.GetInformation(fileInfo).Version;

                AbsoluteFilePath = fileInfo.FullName;
                ActionJson["FileName"] = fileInfo.Name;
                ActionJson["MD5"] = GetMd5FromFile(AbsoluteFilePath);
                ActionJsonUpdated();
            }
        }



        [PropertyOrder(11)]
        public string Version
        {
            get { return (string)ActionJson["Version"]; }
            private set
            {
                ActionJson["Version"] = value;
                ActionJsonUpdated();
            }
        }



        public override void Execute(UpdateProjectEngine updateEngine)
        {
            try
            {
                ActionResult = ActionResult.InProgress;

                var version = Version;
                if (version[0] != 'v')
                    version = "v" + version;

                // find the current version
                var getOsVersionCommand = new GetOsVersionCommand(JniorWebSocket);
                getOsVersionCommand.Log += JniorWebSocket_Log;
                getOsVersionCommand.Execute();

                var versionResponse = getOsVersionCommand.Response;
                OnUpdateInfo(new InformationEventArgs("Version response: " + versionResponse));

                var versionPattern = new Regex(@" = (.*)");
                var versionMatch = versionPattern.Match(versionResponse);
                var currentVersion = versionMatch.Groups[1].Value;
                OnUpdateInfo(new InformationEventArgs(currentVersion + " ?= " + version));
                if (currentVersion.Equals(version))
                {
                    SendUpdate("OS " + Version + " up to date");
                    ActionResult = ActionResult.NotNeeded;
                }
                else
                {
                    //// send the file
                    var fileInfo = new FileInfo(AbsoluteFilePath);

                    var sendFile = new SendFileCommand(JniorWebSocket, fileInfo.FullName, "temp");
                    sendFile.Log += JniorWebSocket_Log;
                    sendFile.Execute();

                    if (sendFile.Completed)
                    {
                        var jrUpdateCommand = new JrUpdateCommand(JniorWebSocket, fileInfo.Name);
                        jrUpdateCommand.Log += JniorWebSocket_Log;
                        jrUpdateCommand.Complete += JrUpdateCommand_Complete;
                        jrUpdateCommand.Execute();
                    }
                    else
                    {
                        throw new Exception("Installation of OS failed.  Unable to upload upd file");
                    }


                    // make sure we are running the new version
                    // find the current version
                    getOsVersionCommand = new GetOsVersionCommand(JniorWebSocket);
                    getOsVersionCommand.Log += JniorWebSocket_Log;
                    getOsVersionCommand.Execute();
                    versionResponse = getOsVersionCommand.Response;
                    OnUpdateInfo(new InformationEventArgs("Updated Version response: " + versionResponse));

                    versionMatch = versionPattern.Match(versionResponse);
                    currentVersion = versionMatch.Groups[1].Value;
                    currentVersion = versionPattern.Match(versionResponse).Groups[1].Value;
                    if (currentVersion.Equals(version))
                    {
                        SendUpdate("Installation of OS complete");
                        ActionResult = ActionResult.Success;
                    }
                    else
                    {
                        throw new Exception("Installation of OS failed.  New version is not correct");
                    }
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
