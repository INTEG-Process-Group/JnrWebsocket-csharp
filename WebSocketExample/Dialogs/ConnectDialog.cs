using Integpg.JniorWebSocket;
using System.Windows.Forms;

namespace WebSocketExample
{
    public partial class ConnectDialog : Form
    {
        public ConnectionProperties ConnectProperties
        {
            get
            {
                var connectProperties = new ConnectionProperties();
                connectProperties.IpAddress = ipTextBox1.IPAddress;
                connectProperties.Port = int.Parse(textBox1.Text);
                connectProperties.IsSecure = checkBox1.Checked;
                ;

                return connectProperties;
            }
            set
            {
                ipTextBox1.IPAddress = value.IpAddress;
                textBox1.Text = value.Port.ToString();
                checkBox1.Checked = value.IsSecure;
            }
        }



        public ConnectDialog()
        {
            InitializeComponent();
        }

    }
}
