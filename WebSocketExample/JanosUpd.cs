using System;
using System.IO;
using System.Text;

namespace WebSocketExample
{
    /**
     * This class helps determine if a given file is a JANOS UPD.  If the file 
     * is a JANOS UPD then it can be parsed for file information.
     */
    public class JanosUpd
    {
        public class JanosFileInformation
        {
            public string Copyright;
            public string Version;
            public string Notes;
        }

        public static bool IsJanosUpd(FileInfo file)
        {
            try
            {
                /* open the file and look at the first 4 bytes.  If "UPD\0" is found 
                 * then it is a valid JANOS UPD. */
                using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    if (br.Read() == 'U' && br.Read() == 'P' && br.Read() == 'D' && br.Read() == 0) return true;
                }
            }
            catch (Exception) { }

            return false;
        }

        public static JanosFileInformation GetInformation(FileInfo file)
        {
            try
            {
                /* make sure it is a valid JANOS UPD */
                if (!IsJanosUpd(file)) throw new ArgumentException("File is not a valid JANOS UPD");

                /* open the file and look at the first 4 bytes.  If "UPD\0" is found 
                 * then it is a valid JANOS UPD. */
                using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    fs.Seek(4, SeekOrigin.Begin);

                    /* create new file information struct */
                    JanosFileInformation janosFileInfo = new JanosFileInformation();
                    janosFileInfo.Copyright = ReadCString(br);
                    janosFileInfo.Version = ReadCString(br);
                    janosFileInfo.Notes = ReadCString(br);

                    return janosFileInfo;
                }
            }
            catch (Exception) { }

            return null;
        }

        private static string ReadCString(BinaryReader br)
        {
            StringBuilder sb = new StringBuilder();
            char c;
            while ((c = br.ReadChar()) != '\0')
            {
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
