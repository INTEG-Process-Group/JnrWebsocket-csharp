namespace Integpg.JniorWebSocket.Messages
{
    public class FileList : MetaMessage
    {
        public FileList(string folder) : base("File List")
        {
            this["Folder"] = folder;
        }
    }
}
