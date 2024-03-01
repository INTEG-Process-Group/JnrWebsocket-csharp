using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using WebSocketExample.Dialogs;

namespace WebSocketExample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // find a jrup file if it exists
            var directoryInfo = new DirectoryInfo(Application.StartupPath);
            var jrupFiles = directoryInfo.GetFiles("*.jrup");
            //var updateProjectFilePath = @"C:\Users\kcloutier\Documents\Visual Studio 2015\Projects\WebSocketExample\WebSocketExample\bin\Debug\install-janos-1.6.3-rc.jrup";
            if (null != jrupFiles && 1 == jrupFiles.Length)
            {
                Application.Run(new PublishUpdateProjectDialog(jrupFiles[0].FullName));
            }
            else
            {
                Application.Run(new Form1());
            }
        }



        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var a1 = Assembly.GetExecutingAssembly();

                var resourceName = new AssemblyName(args.Name).Name + ".dll";
                var manifestResourceNames = a1.GetManifestResourceNames();
                var resource = Array.Find(manifestResourceNames, element => element.EndsWith(resourceName));

                if (null != resource)
                {
                    using (var stream = a1.GetManifestResourceStream(resource))
                    {
                        var assemblyData = new byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        return Assembly.Load(assemblyData);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
