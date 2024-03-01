using Integpg.JniorWebSocket;
using Integpg.JniorWebSocket.Messages;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MultipleJniors
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        private void addJNIORToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var connectionDialog = new ConnectDialog();
            if (DialogResult.OK == connectionDialog.ShowDialog(this))
            {
                var connectionProperties = connectionDialog.ConnectProperties;
                var jniorConnection = new JniorConnection(connectionProperties);
                AddJnior(jniorConnection);

                jniorConnection.WebSocket.Connect();
            }
        }



        private void AddJnior(JniorConnection jniorConnection)
        {
            // add the jnior node
            var tnJnior = new JniorTreeNode(jniorConnection);
            tnJnior.Tag = jniorConnection;
            tvwJniors.Nodes.Add(tnJnior);

            // make sure the jnior node is expanded 
            tnJnior.Expand();

            tvwJniors.SelectedNode = tnJnior;
        }



        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            var jniorConnection = null as JniorConnection;

            // get the selected node and make sure there was actually one 
            // selected before continuing
            var tnSelected = tvwJniors.SelectedNode;
            if (tnSelected != null)
            {
                // get the jnior connection object
                if (tnSelected.Tag is JniorConnection)
                {
                    jniorConnection = tnSelected.Tag as JniorConnection;
                }
            }

            connectToolStripMenuItem.Enabled = null != jniorConnection && !jniorConnection.WebSocket.IsOpened;
            disconnectToolStripMenuItem.Enabled = null != jniorConnection && jniorConnection.WebSocket.IsOpened;
            removeToolStripMenuItem.Enabled = null != jniorConnection;
            pulseROUT1ToolStripMenuItem.Enabled = null != jniorConnection && jniorConnection.WebSocket.IsOpened;
        }



        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get the selected node and make sure there was actually one 
            // selected before continuing
            var tnSelected = tvwJniors.SelectedNode;
            if (tnSelected == null)
                return;

            if (tnSelected.Tag is JniorConnection)
            {
                // get the jnior connection object
                var jniorConnection = tnSelected.Tag as JniorConnection;
                jniorConnection.WebSocket.Connect();
            }
        }



        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get the selected node and make sure there was actually one 
            // selected before continuing
            var tnSelected = tvwJniors.SelectedNode;
            if (tnSelected == null)
                return;

            if (tnSelected.Tag is JniorConnection)
            {
                // get the jnior connection object
                var jniorConnection = tnSelected.Tag as JniorConnection;
                jniorConnection.WebSocket.Close();
            }
        }



        private void pulseROUT1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get the selected node and make sure there was actually one 
            // selected before continuing
            var tnSelected = tvwJniors.SelectedNode;
            if (tnSelected == null)
                return;

            if (tnSelected.Tag is JniorConnection)
            {
                // get the jnior connection object
                var jniorConnection = tnSelected.Tag as JniorConnection;


                /**
                 * So here we have an option.  We can define the entire JSON structure like so,
                 * 
                 * var json = @"{
                 *   Message: 'Control',
                 *   Command: 'Close',
                 *   Channel: 1,
                 *   Duration: 1000
                 * }";
                 * var pulseOutput = JObject.Parse(json);
                 * 
                 *          or
                 * 
                 * var pusleOutput = JObject.FromObject(new
                 * {
                 *     Message = "Control",
                 *     Command = "Close",
                 *     Channel = 1,
                 *     Duration = 1000
                 * });
                 *
                 * or build off of one of the base objects I have created.  I havent created a 
                 * PulseOutput Command yet but by looking at the WebSocket document online I 
                 * see that it would be a CloseOutput object with an added paramter for 
                 * Duration.  To build the CloseOutput is use the ControlOutput with the 
                 * "Close" parameter and  the channel.  I then add the Duration
                 * 
                 * var pulseOutput = new ControlOutput("Close", 1);
                 * pulseOutput["Duration"] = 1000;
                 * 
                 * This allos us to extend the messages sent to the JNIOR without them needing 
                 * to already be in the library.
                 */

                var pulseOutput = new ControlOutput("Close", 1);
                pulseOutput["Duration"] = 1000;

                jniorConnection.WebSocket.Send(pulseOutput);
            }
        }



        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            disconnectToolStripMenuItem_Click(sender, e);

            var tnSelected = tvwJniors.SelectedNode;
            if (null != tnSelected)
            {
                if (tnSelected.Tag is JniorConnection)
                {
                    tvwJniors.Nodes.Remove(tnSelected);
                }
            }
        }



        private void readJniorsyslogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tnSelected = tvwJniors.SelectedNode;
            if (null != tnSelected)
            {
                if (tnSelected.Tag is JniorConnection)
                {
                    var jniorConnection = tnSelected.Tag as JniorConnection;

                    var textBox = new JniorsysLogViewer(jniorConnection);
                    var tabPage = new TabPage("jniorsys.log");
                    tabPage.Controls.Add(textBox);

                    tabControl1.TabPages.Add(tabPage);

                    textBox.ReadFile();
                }
            }
        }




    }
}
