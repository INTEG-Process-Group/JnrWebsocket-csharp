using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using WebSocketExample.Actions;

namespace WebSocketExample.Editors
{
    class UdpFileSelectEditor : System.Drawing.Design.UITypeEditor
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
                ofd.Filter = "JANOS UPD|*.upd";

                if (null != context.Instance)
                {
                    var installOsAction = context.Instance as InstallOsAction;
                    if (null != installOsAction.AbsoluteFilePath)
                    {
                        var file = new System.IO.FileInfo(installOsAction.AbsoluteFilePath);
                        ofd.InitialDirectory = file.Directory.ToString();
                    }
                }

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    return ofd.FileName;
                }
            }
            return value;
        }

    }
}
