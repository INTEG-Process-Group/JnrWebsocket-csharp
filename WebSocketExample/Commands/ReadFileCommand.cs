using Integpg.JniorWebSocket;
using Integpg.JniorWebSocket.Messages;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketExample.Commands
{
    public class ReadFileCommand : CommandBase
    {
        public event EventHandler Done;


        public byte[] FileContent { get; private set; }

        public string RemoteFile { get; private set; }

        public JObject FileInfoJson { get; set; }

        private string _localFolder;
        private ManualResetEvent _fileReceivedWait = new ManualResetEvent(false);
        private HttpWebRequest _httpWebRequest;



        public ReadFileCommand(JniorWebSocket jniorWebsocket, string remoteFile, string localFolder) : base(jniorWebsocket)
        {
            RemoteFile = remoteFile;
            _localFolder = localFolder;

            _jniorWebsocket.Websocket.MessageReceived += Websocket_MessageReceived;
        }



        private void Websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var json = JObject.Parse(e.Message);
                var message = json["Message"].ToString();
                if ("File Read Response".Equals(message))
                {
                    //OnLog("   " + e.Message);

                    var status = (string)json["Status"];
                    if ("Ready".Equals(status))
                    {
                        var filename = (string)json["File"];
                        filename = filename.Replace("/", "\\");

                        var requestID = (int)json["RequestID"];
                        var start = DateTime.Now;
                        DownloadAsync("http://" + _jniorWebsocket.Host + "/query.cgi?request=" + requestID, _localFolder + filename);
                        var elapsed = DateTime.Now - start;
                        //OnLog("took " + elapsed);

                        _fileReceivedWait.Set();
                    }
                    else
                    {
                        _fileReceivedWait.Set();
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
                var requestID = (long)(new Random().NextDouble() * int.MaxValue);
                // read the file
                var fileRead = new FileRead(RemoteFile, requestID);
                _jniorWebsocket.Send(fileRead);

                var success = _fileReceivedWait.WaitOne(30000);

                if (!success)
                {
                    if (null != _httpWebRequest)
                    {
                        _httpWebRequest.Abort();
                        OnError(new Exception("httpWebRequest aborted"));
                    }
                    else
                    {
                        OnError(new Exception("_httpWebRequest should never be null"));
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }



        public void DownloadAsync(string requestUri, string filename)
        {
            if (requestUri == null)
            {
                throw new ArgumentNullException("requestUri");
            }

            DownloadAsync(new Uri(requestUri), filename);
        }



        public async void DownloadAsync(Uri requestUri, string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            try
            {
                var file = new FileInfo(filename);
                if (!file.Directory.Exists)
                {
                    OnLog("Create " + file.Directory + "\r\n");
                    file.Directory.Create();
                }

                //var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
                _httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
                _httpWebRequest.Method = "Get";
                _httpWebRequest.Timeout = 10000;
                OnLog("   GET " + requestUri + ": " + RemoteFile);

                var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                _httpWebRequest.CachePolicy = noCachePolicy;

                var httpWebResponse = (HttpWebResponse)_httpWebRequest.GetResponse();
                if (HttpStatusCode.OK == httpWebResponse.StatusCode)
                {
                    await HandleDownloadResponseAsync(file, httpWebResponse);

                    //using (var sr = httpWebResponse.GetResponseStream())
                    //using (var sw = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 16, true))
                    //{
                    //    sr.BaseStream.CopyTo(sw);
                    //}

                    //var lastModifiedHeader = response.Headers["Last-Modified"] as string;
                    //var lastModifiedDate = DateTime.Parse(lastModifiedHeader);
                    //file.LastWriteTimeUtc = lastModifiedDate;

                    //OnLog(" Downloaded " + _remoteFile);
                }
                else
                {
                    OnError(new Exception(RemoteFile + ": " + httpWebResponse.StatusDescription));
                }
            }
            catch (Exception ex)
            {
                OnError(new Exception(ex.Message + ": " + requestUri + ": " + RemoteFile));
            }

        }



        private async Task HandleDownloadResponseAsync(FileInfo file, HttpWebResponse httpWebResponse)
        {
            using (var sr = httpWebResponse.GetResponseStream())
            using (var sw = new FileStream(file.FullName, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 16, true))
            {
                await sr.CopyToAsync(sw);
            }

            var lastModifiedHeader = httpWebResponse.Headers["Last-Modified"] as string;
            var lastModifiedDate = DateTime.Parse(lastModifiedHeader);
            file.LastWriteTimeUtc = lastModifiedDate;

            OnLog(" Downloaded " + RemoteFile);

            Done?.Invoke(this, EventArgs.Empty);
        }



        //public Task DownloadAsync(string requestUri, string filename)
        //{
        //    if (requestUri == null)
        //        throw new ArgumentNullException("requestUri");

        //    return DownloadAsync(new Uri(requestUri), filename);
        //}
        //private static HttpClient httpClient = new HttpClient();


        //public async Task DownloadAsync(Uri requestUri, string filename)
        //{
        //    try
        //    {
        //        if (filename == null)
        //            throw new ArgumentNullException("filename");

        //        var file = new FileInfo(filename);
        //        if (!file.Directory.Exists)
        //        {
        //            file.Directory.Create();
        //        }

        //        OnLog(requestUri + ": " + _remoteFile);

        //        //using (var httpClient = new HttpClient())
        //        //{
        //        var request = await httpClient.GetAsync(requestUri);
        //        Console.WriteLine(request.StatusCode);

        //        using (Stream contentStream = await (request.Content.ReadAsStreamAsync()),
        //                     stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 16, true))
        //        {
        //            await contentStream.CopyToAsync(stream);
        //        }
        //        if (request.StatusCode == HttpStatusCode.OK)
        //        {
        //            var lastModified = request.Content.Headers.LastModified.Value;
        //            file.LastWriteTimeUtc = lastModified.DateTime;
        //            OnLog("downloaded " + ++DowloadCount + ": " + _remoteFile + "\r\n");
        //        }
        //        else
        //        {
        //            OnLog(_remoteFile + ": " + request.ReasonPhrase + "\r\n");
        //        }


        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        OnError(new Exception(ex.Message + ": " + requestUri + ": " + _remoteFile));
        //    }
        //}

        //private static int DowloadCount = 0;
    }
}
