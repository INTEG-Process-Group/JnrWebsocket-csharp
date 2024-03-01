using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Integpg.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketExample.Actions;
using System.Security.Cryptography;

namespace WebSocketExample.Dialogs
{
    public partial class EditPropertiesDlg : Form
    {
        private static readonly NLog.Logger Log = LogDictionary.GetLog("EditPropertiesDialog");

        public string Filename { get; private set; }
        private UpdateProject _updatePackage;



        public EditPropertiesDlg()
        {
            InitializeComponent();
        }



        public void SetFilename(string filename)
        {
            Filename = filename;

            if (Filename != null)
            {
                _updatePackage = new UpdateProject(Filename);
                _updatePackage.ConfigurationUpdated += _updatePackage_ConfigurationUpdated;
                _updatePackage.Open();

                PackageConfigurationUpdated();
            }
            else
            {
                _updatePackage = new UpdateProject();
                _updatePackage.ConfigurationUpdated += _updatePackage_ConfigurationUpdated;
            }
        }



        private void EditPropertiesDlg_Shown(object sender, EventArgs e)
        {
        }



        void _updatePackage_ConfigurationUpdated(object sender, EventArgs e)
        {
            PackageConfigurationUpdated();
        }



        public void PackageConfigurationUpdated()
        {
            lvwSteps.Items.Clear();
            foreach (var actionBase in _updatePackage.Steps)
            {
                var lvi = lvwSteps.Items.Add(actionBase.Name);
                lvi.Tag = actionBase;
                if (IsFileChanged(actionBase))
                {
                    lvi.ForeColor = System.Drawing.Color.Blue;
                }
            }

            txtJson.Text = GetPackageJson().ToString(Formatting.Indented, null);
            txtJson.SelectionLength = 0;
        }



        private bool IsFileChanged(ActionBase actionBase)
        {
            if (actionBase is UploadFileAction)
            {
                var uploadFileAction = actionBase as UploadFileAction;
                var absoluteFileLocation = uploadFileAction.AbsoluteFilePath;
                var localMd5 = GetMd5FromFile(absoluteFileLocation);
                return !localMd5.Equals(uploadFileAction.MD5);
            }
            return false;
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



        private JToken GetPackageJson()
        {
            var instructionJson = new JObject();
            var actionsJson = new JArray();
            foreach (var actionBase in _updatePackage.Steps)
            {
                actionsJson.Add(actionBase.ActionJson);
            }
            instructionJson["Actions"] = actionsJson;
            return instructionJson;
        }



        private void btnSave_Click(object sender, EventArgs e)
        {
            Log.Debug("Check for temp directory");
            //if (Directory.Exists("temp")) Directory.Delete("temp", true);
            if (!Directory.Exists("temp"))
            {
                Log.Debug("Create temp directory");
                Directory.CreateDirectory("temp");
            }

            Log.Debug("Get all file in temp");
            var di = new DirectoryInfo("temp");
            var fis = di.GetFiles();
            var tempFileList = new List<string>();
            foreach (var fi in fis)
                tempFileList.Add(fi.Name.ToLower());
            tempFileList.Remove("instructions.json");
            Log.Debug("Found " + tempFileList.Count + " files");

            Log.Debug("Go through each action and gather the file into the temp directory");
            foreach (var actionBase in _updatePackage.Steps)
            {
                //if (actionBase.AbsoluteFilePath != null)
                //{
                //    var fileInfo = new FileInfo(actionBase.AbsoluteFilePath);
                //    var newPath = Path.Combine("temp", fileInfo.Name);
                //    var newFileInfo = new FileInfo(newPath);
                //    try
                //    {
                //        if (!fileInfo.FullName.Equals(newFileInfo.FullName, StringComparison.CurrentCultureIgnoreCase))
                //            File.Copy(fileInfo.FullName, newFileInfo.FullName, true);


                //    }
                //    catch (Exception) { }
                //}

                var installOsAction = actionBase as InstallOsAction;
                if (installOsAction != null)
                {
                    if (tempFileList.Contains<string>(installOsAction.FileName.ToLower()))
                    {
                        tempFileList.Remove(installOsAction.FileName.ToLower());
                    }
                }

                //var transferZipAction = actionBase as TransferZipAction;
                //if (transferZipAction != null)
                //{
                //    if (tempFileList.Contains<string>(transferZipAction.FileName.ToLower()))
                //    {
                //        tempFileList.Remove(transferZipAction.FileName.ToLower());
                //    }
                //}
            }

            Log.Debug(tempFileList.Count + " file that are no longer needed");
            foreach (var tempFile in tempFileList)
            {
                File.Delete(Path.Combine("temp", tempFile));
            }

            Log.Debug("Write instructions json file");
            File.WriteAllText("temp\\instructions.json", GetPackageJson().ToString(Formatting.Indented, null));

            Log.Debug("Prompt user for zip file save location");
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = "zip";
            saveFileDialog.Filter = "Jnior Update Project (*.jrup)|*.jrup";

            if (null != Filename)
            {
                var currentFile = new FileInfo(Filename);
                saveFileDialog.InitialDirectory = currentFile.Directory.ToString();
            }

            if (Filename != null)
            {
                saveFileDialog.FileName = Filename;
            }

            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                Log.Debug("Create zip file");
                var fsOut = File.Create(saveFileDialog.FileName);
                var zipStream = new ZipOutputStream(fsOut);

                var folderName = Path.Combine(Application.StartupPath, "temp");

                // This setting will strip the leading part of the folder path in the entries, to
                // make the entries relative to the starting folder.
                // To include the full path for each entry up to the drive root, assign folderOffset = 0.
                var folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

                CompressFolder(folderName, zipStream, folderOffset);

                zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
                zipStream.Close();

                Filename = saveFileDialog.FileName;

                DialogResult = DialogResult.OK;
            }
        }



        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {

            string[] files = Directory.GetFiles(path);

            foreach (string filename in files)
            {

                FileInfo fi = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }



        private void installOSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var actionBase = InstallOsAction.CreateAction("Install OS");
            _updatePackage.AddStep(actionBase);
            PackageConfigurationUpdated();
            propertyGrid1.SelectedObject = actionBase;
        }



        private void telnetCommandsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var actionBase = TelnetCommandsAction.CreateAction("TelnetCommands");
            _updatePackage.AddStep(actionBase);
            PackageConfigurationUpdated();
            lvwSteps.Items[lvwSteps.Items.Count - 1].Selected = true;
        }



        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedIndex = lvwSteps.SelectedItems[0].Index;
                var actionBase = _updatePackage.Steps[selectedIndex];
                _updatePackage.Steps.Remove(actionBase);
                _updatePackage.Steps.Insert(selectedIndex - 1, actionBase);
                PackageConfigurationUpdated();
                lvwSteps.Items[selectedIndex - 1].Selected = true;
            }
            catch (Exception ex) { }
        }



        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedIndex = lvwSteps.SelectedItems[0].Index;
                var actionBase = _updatePackage.Steps[selectedIndex];
                _updatePackage.Steps.Remove(actionBase);
                _updatePackage.Steps.Insert(selectedIndex + 1, actionBase);
                PackageConfigurationUpdated();
                lvwSteps.Items[selectedIndex + 1].Selected = true;
            }
            catch (Exception ex) { }
        }



        private void btnRemoveAction_Click(object sender, EventArgs e)
        {
            var selectedIndex = lvwSteps.SelectedItems[0].Index;
            var actionBase = _updatePackage.Steps[selectedIndex];
            _updatePackage.Steps.Remove(actionBase);
            PackageConfigurationUpdated();
            if (selectedIndex < lvwSteps.Items.Count)
                lvwSteps.Items[selectedIndex].Selected = true;
            else if (lvwSteps.Items.Count > 0)
                lvwSteps.Items[selectedIndex - 1].Selected = true;
            else
                lvwSteps.SelectedItems.Clear();
        }



        private void rebootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var actionBase = RebootAction.CreateAction("Reboot");
            _updatePackage.AddStep(actionBase);
            PackageConfigurationUpdated();
        }



        private void uploadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var actionBase = UploadFileAction.CreateAction("UploadFile");
            _updatePackage.AddStep(actionBase);
            PackageConfigurationUpdated();
            propertyGrid1.SelectedObject = actionBase;
        }



        private void registryIngestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var actionBase = RegistryIngestAction.CreateAction("RegistryIngest");
            _updatePackage.AddStep(actionBase);
            PackageConfigurationUpdated();
            propertyGrid1.SelectedObject = actionBase;
        }



        private void lvwSteps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (0 != lvwSteps.SelectedItems.Count)
            {
                var selectedListItem = lvwSteps.SelectedItems[0];
                var selectedIndex = selectedListItem.Index;
                propertyGrid1.SelectedObject = selectedIndex >= 0 ? _updatePackage.Steps[selectedIndex] : null;
                btnMoveUp.Enabled = selectedIndex > 0;
                btnMoveDown.Enabled = selectedIndex + 1 < lvwSteps.Items.Count;

                btnUpdateReference.Visible = (System.Drawing.Color.Blue == selectedListItem.ForeColor);
            }
        }



        private void btnUpdateReference_Click(object sender, EventArgs e)
        {
            if (0 != lvwSteps.SelectedItems.Count)
            {
                var selectedListItem = lvwSteps.SelectedItems[0];
                var actionBase = selectedListItem.Tag as ActionBase;
                var fileInfo = new FileInfo(actionBase.AbsoluteFilePath);
                var newPath = Path.Combine("temp", fileInfo.Name);
                var newFileInfo = new FileInfo(newPath);
                try
                {
                    if (!fileInfo.FullName.Equals(newFileInfo.FullName, StringComparison.CurrentCultureIgnoreCase))
                        File.Copy(fileInfo.FullName, newFileInfo.FullName, true);

                    if (actionBase is UploadFileAction)
                    {
                        var uploadFileAction = actionBase as UploadFileAction;
                        uploadFileAction.MD5 = GetMd5FromFile(newFileInfo.FullName);
                        PackageConfigurationUpdated();
                    }
                }
                catch (Exception) { }
            }
        }
    }
}
