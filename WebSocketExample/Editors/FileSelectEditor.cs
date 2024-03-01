using System;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace WebSocketExample.Editors
{
    class FileSelectEditor : System.Drawing.Design.UITypeEditor
    {
        // Indicates whether the UITypeEditor provides a form-based (modal) dialog, 
        // drop down dialog, or no UI outside of the properties window.
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }



        // Displays the UI for value selection.
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            // Uses the IWindowsFormsEditorService to display a 
            // drop-down UI in the Properties window.
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var fileInfo = new FileInfo(ofd.FileName);
                    var newPath = Path.Combine("temp", fileInfo.Name);
                    var newFileInfo = new FileInfo(newPath);
                    try
                    {
                        if (!fileInfo.FullName.Equals(newFileInfo.FullName, StringComparison.CurrentCultureIgnoreCase))
                            File.Copy(fileInfo.FullName, newFileInfo.FullName, true);
                    }
                    catch (Exception) { }
                    return ofd.FileName;
                }
            }
            return value;
        }

    }
}
