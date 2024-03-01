using System;
using System.IO;

namespace Integpg.JniorWebSocket.Messages
{
    public class FileWrite : MetaMessage
    {
        public FileWrite(string destinationFileName, FileInfo fileInfo) : base("File Write")
        {
            var fileBytes = File.ReadAllBytes(fileInfo.FullName);

            Init(destinationFileName, fileInfo, fileBytes, fileBytes.Length);
        }



        public FileWrite(string destinationFileName, FileInfo fileInfo, byte[] fileBytes, int count) : this(destinationFileName, fileInfo)
        {
            Init(destinationFileName, fileInfo, fileBytes, count);
        }



        private void Init(string destinationFileName, FileInfo fileInfo, byte[] fileBytes, int count)
        {
            if (!destinationFileName.StartsWith("/"))
                destinationFileName = "/" + destinationFileName;
            this["File"] = destinationFileName;

            this["Size"] = count;

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var listWriteTime = fileInfo.LastWriteTime.ToUniversalTime() - epoch;
            this["Mod"] = (long)listWriteTime.TotalMilliseconds;
            this["Encoding"] = "base64";

            var base64File = Convert.ToBase64String(fileBytes, 0, count);
            this["Data"] = base64File;
        }
    }
}
