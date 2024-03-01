using Integpg.JniorWebSocket;
using Integpg.JniorWebSocket.Messages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace WebSocketExample.Commands
{
    public class SendFileCommand : CommandBase
    {
        private string _filename;

        public string RemoteFolder { get; private set; }

        private string _remoteName;



        public SendFileCommand(JniorWebSocket jniorWebsocket, string filename, string remoteFolder) : base(jniorWebsocket)
        {
            _filename = filename;
            RemoteFolder = remoteFolder;

            _jniorWebsocket.Websocket.MessageReceived += Websocket_MessageReceived;
        }



        public SendFileCommand(JniorWebSocket jniorWebsocket, string filename, string remoteFolder, string remoteName) : base(jniorWebsocket)
        {
            _filename = filename;
            RemoteFolder = remoteFolder;
            _remoteName = remoteName;

            _jniorWebsocket.Websocket.MessageReceived += Websocket_MessageReceived;
        }



        private void Websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var json = JObject.Parse(e.Message);
                var message = json["Message"].ToString();
                if ("File Write Response".Equals(message))
                {
                    //OnLog("   " + e.Message);

                    var status = (string)json["Status"];
                    if ("Succeed".Equals(status))
                    {
                        var filename = (string)json["File"];
                        filename = filename.Replace("/", "\\");


                    }
                    else if ("Fail".Equals(status))
                    {
                        // we failed.
                        throw new Exception("File Write Failed");
                    }

                    //var fileData = json["Data"].ToString();
                    //FileContent = Convert.FromBase64String(fileData);
                    //_fileReceivedWait.Set();
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }



        protected override void CommandFinished()
        {
            _jniorWebsocket.Websocket.MessageReceived -= Websocket_MessageReceived;
        }



        protected override void DoCommand()
        {
            try
            {
                var fileInfo = new FileInfo(_filename);

                OnLog(string.Format("sending {0} ({1} bytes)", _filename, fileInfo.Length));

                // lets make sure the directory exists that we are going to be sending to
                var checkDirectoryCommand = new TelnetCommand(_jniorWebsocket, "ls " + RemoteFolder);
                checkDirectoryCommand.Log += TelnetCommandLog;
                checkDirectoryCommand.Execute();
                OnLog(checkDirectoryCommand.Response);
                if (checkDirectoryCommand.Response.Contains("No such file or directory"))
                {
                    OnLog(string.Format("{0} does not exist", RemoteFolder));

                    // create a stack to place out directoies that need to be created.  we need to traverse 
                    // toward the root and create each parent diretory until the full path has been created
                    var stack = new Stack<string>();

                    // capture our remote folder.  this local variable will be modified as we traverse 
                    // toward the root.  we dont want to modify the path we wish to upload to
                    var remoteFolder = RemoteFolder;
                    // trim off any trailing slashes
                    remoteFolder = remoteFolder.TrimEnd(new char[] { '/' });

                    do
                    {
                        // the directory does not exist.  we must create it.
                        var makeDirectoryCommand = new TelnetCommand(_jniorWebsocket, "mkdir " + remoteFolder);
                        makeDirectoryCommand.Log += TelnetCommandLog;
                        makeDirectoryCommand.Execute();
                        OnLog(makeDirectoryCommand.Response);

                        if (makeDirectoryCommand.Response.ToLower().Contains("cannot create"))
                        {
                            OnLog(makeDirectoryCommand.Response + ": " + remoteFolder);

                            // push this location
                            stack.Push(remoteFolder);

                            // get the parent to this folder and try to create that
                            var lastSlashIndex = remoteFolder.LastIndexOf('/');
                            if (-1 == lastSlashIndex)
                            {
                                // no more directories!  this is a big issue
                                break;
                            }
                            remoteFolder = remoteFolder.Substring(0, lastSlashIndex);
                        }
                        else
                        {
                            // a parent directory was created.  break out of this loop and process the stack
                            OnLog(remoteFolder + " was successfuly created");
                            break;
                        }
                    } while (true);

                    // if we have added directories to the stack then we must go back through the stack 
                    // and try to create them again since we were not able to create them the first time.
                    while (0 != stack.Count)
                    {
                        remoteFolder = stack.Pop();

                        var makeDirectoryCommand = new TelnetCommand(_jniorWebsocket, "mkdir " + remoteFolder);
                        makeDirectoryCommand.Log += TelnetCommandLog;
                        makeDirectoryCommand.Execute();
                        OnLog(makeDirectoryCommand.Response);

                        if (makeDirectoryCommand.Response.ToLower().Contains("cannot create"))
                        {
                            // shit. w we still cant create it.  WHY!!!
                        } else
                        {
                            OnLog(remoteFolder + " was successfuly created");
                        }
                    }

                    // recheck to see that our directory has been made.
                    checkDirectoryCommand = new TelnetCommand(_jniorWebsocket, "ls " + RemoteFolder);
                    checkDirectoryCommand.Log += TelnetCommandLog;
                    checkDirectoryCommand.Execute();
                    OnLog(checkDirectoryCommand.Response);
                    if (checkDirectoryCommand.Response.Contains("No such file or directory"))
                    {
                        // this is no good
                        throw new Exception(checkDirectoryCommand.Response + ".  Unable to create directory");
                    }
                }

                // send the file
                using (var fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                using (var br = new BinaryReader(fs))
                {
                    var buffer = new byte[8192];
                    var bytesRead = 0;
                    var totalBytesProcessed = 0;
                    while (0 < (bytesRead = br.Read(buffer, 0, buffer.Length)))
                    {
                        if (_disconnected)
                        {
                            throw new Exception("Unit has disconnected.  Cannot send file");
                        }

                        var remoteName = _remoteName ?? fileInfo.Name;
                        var fileWrite = new FileWrite(RemoteFolder + "/" + remoteName, fileInfo, buffer, bytesRead);
                        if (0 < totalBytesProcessed)
                            fileWrite["Append"] = true;
                        totalBytesProcessed += bytesRead;
                        _jniorWebsocket.Send(fileWrite);

                        _jniorWebsocket.WaitForMeta((string)fileWrite["Meta"]);

                        double percentage = totalBytesProcessed / (double)fileInfo.Length * 100;
                        OnLog(string.Format("{0:N0} / {1:N0} = {2:F1}% transferred", totalBytesProcessed, fileInfo.Length, percentage));
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }



        private void TelnetCommandLog(object sender, LogEventArgs e)
        {
            OnLog(e.Message);
        }
    }
}
