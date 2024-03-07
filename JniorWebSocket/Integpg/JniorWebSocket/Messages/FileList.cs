namespace Integpg.JniorWebSocket.Messages
{
    public class FileList : JniorMessage
    {
        public FileList(string folder) : base("File List")
        {
            this["Folder"] = folder;
            this["Meta"] = new MetaMessage();
        }
    }
}
