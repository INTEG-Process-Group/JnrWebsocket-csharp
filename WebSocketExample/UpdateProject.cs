using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Integpg.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using WebSocketExample.Actions;

namespace WebSocketExample
{
    public class UpdateProject
    {
        private static readonly NLog.Logger Log = LogDictionary.GetLog("UpdateProject");

        public event EventHandler ProjectOpened;
        public event EventHandler ConfigurationUpdated;
        public event EventHandler<ErrorEventArgs> Error;



        public string Name { get; private set; }
        public string Filename { get; private set; }
        public string InstructionFile { get; private set; }
        public JObject InstructionsJson { get; private set; }
        public List<ActionBase> Steps { get { return _steps; } }



        private ZipInputStream _zipInputStream;
        private List<ActionBase> _steps = new List<ActionBase>();



        public UpdateProject()
        {
        }



        public UpdateProject(string filename)
        {
            var file = new FileInfo(filename);
            Name = file.Name;
            Filename = filename;
        }



        public void Open()
        {
            if (Directory.Exists("temp"))
            {
                Log.Info("Delete Temp Directory");
                Directory.Delete("temp", true);
            }

            var fileStream = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            _zipInputStream = new ZipInputStream(fileStream);
            Log.Info(Filename + "opened");

            var zipEntry = _zipInputStream.GetNextEntry();
            while (zipEntry != null)
            {
                var entryFileName = zipEntry.Name;
                var tempFile = Path.Combine("temp", entryFileName);
                var tempDirectory = Path.GetDirectoryName(tempFile);
                if (tempDirectory != null && !Directory.Exists(tempDirectory)) Directory.CreateDirectory(tempDirectory);

                using (var outputFile = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                {
                    UnpackFromZip(outputFile);
                }

                zipEntry = _zipInputStream.GetNextEntry();
            }

            InstructionFile = "temp\\instructions.json";
            LoadInstructions(InstructionFile);

            ProjectOpened?.Invoke(this, EventArgs.Empty);
        }




        private void UnpackFromZip(Stream stream)
        {
            var buffer = new byte[4096];
            using (stream)
            {
                StreamUtils.Copy(_zipInputStream, stream, buffer);
            }
        }



        private void LoadInstructions(string instructionsFile)
        {
            var fileText = File.ReadAllText(instructionsFile);
            InstructionsJson = JObject.Parse(fileText);
            GetSteps();
        }



        public void AddStep(ActionBase actionBase)
        {
            actionBase.ConfigurationUpdated += ConfigurationUpdated;
            Steps.Add(actionBase);
        }



        private void GetSteps()
        {
            try
            {
                Log.Info("Get steps from instruction file");

                foreach (var actionJson in InstructionsJson["Actions"])
                {
                    var actionBase = ActionBaseFactory.CreateAction(actionJson);
                    actionBase.GetSteps(ref _steps);
                    actionBase.ConfigurationUpdated += ConfigurationUpdated;
                }

                Log.Info(Steps.Count + " steps loaded");
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, new ErrorEventArgs(ex));
            }
        }

    }
}
