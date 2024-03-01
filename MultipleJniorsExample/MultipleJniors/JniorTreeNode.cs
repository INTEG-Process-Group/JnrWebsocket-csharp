using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace MultipleJniors
{
    class JniorTreeNode : TreeNode
    {
        private JniorConnection _jniorConnection;

        private TreeNode _connectionStatusNode;
        private TreeNode _authenticationStatusNode;
        private TreeNode _inputsStatusNode;
        private TreeNode _outputsStatusNode;
        private TreeNode _catJniorsysLogNode;



        public JniorTreeNode(JniorConnection jniorConnection)
        {
            _jniorConnection = jniorConnection;
            Text = jniorConnection.WebSocket.Host + ":" + jniorConnection.WebSocket.Port;

            jniorConnection.WebSocket.Connected += WebSocket_Connected;
            jniorConnection.WebSocket.Disconnected += WebSocket_Disconnected;
            _connectionStatusNode = Nodes.Add("Connection - Waiting...");

            jniorConnection.WebSocket.Authorized += WebSocket_Authorized;
            _authenticationStatusNode = Nodes.Add("Authentication - Waiting...");

            jniorConnection.WebSocket.MessageReceived += WebSocket_MessageReceived;
            _inputsStatusNode = Nodes.Add("Inputs - Waiting...");
            _outputsStatusNode = Nodes.Add("Outputs - Waiting...");

            _catJniorsysLogNode = Nodes.Add("Cat Jniorsys.log");
            _catJniorsysLogNode.Tag = new TextBox();
        }



        private void WebSocket_Connected(object sender, System.EventArgs e)
        {
            JniorConnection_ConnectionStatusUpdate(sender, e);
        }



        private void WebSocket_Disconnected(object sender, System.EventArgs e)
        {
            JniorConnection_ConnectionStatusUpdate(sender, e);
        }



        private void JniorConnection_ConnectionStatusUpdate(object sender, System.EventArgs e)
        {
            TreeView.Invoke((MethodInvoker)delegate ()
            {
                _connectionStatusNode.Text = "Connection - " + (_jniorConnection.WebSocket.IsOpened ? "Connected" : "NOT Connected");
            });
        }



        private void WebSocket_Authorized(object sender, System.EventArgs e)
        {
            TreeView.Invoke((MethodInvoker)delegate ()
            {
                _authenticationStatusNode.Text = "Authentication - " + (_jniorConnection.WebSocket.IsAuthenticated ? "Authenticated" : "NOT Authenticated");
            });
        }



        private void WebSocket_MessageReceived(object sender, Integpg.JniorWebSocket.MessageReceivedEventArgs e)
        {
            // grab input 1 counter value
            var json = JObject.Parse(e.Message);

            var message = json["Message"].ToString();
            if ("Monitor".Equals(message))
            {
                HandleMonitorMessage(json);
            }

        }



        private void HandleMonitorMessage(JObject json)
        {
            // get the model number.  this will be used to determine how many digits to show 
            // in the masks
            var model = (int)json["Model"];
            var inputCount = (414 == model) ? 12 : (412 == model) ? 4 : 8;
            var outputCount = (414 == model) ? 4 : (412 == model) ? 12 : 8;

            // get the inputs.  it will be an array of json objects
            var inputsArray = json["Inputs"] as JArray;

            // loop through the input array and determine the input state mask
            var inputsMask = 0;
            for (var i = 0; i < inputsArray.Count; i++)
            {
                var input = inputsArray[i];
                var state = (int)input["State"];
                if (state == 1)
                {
                    inputsMask |= (1 << i);
                }
            }
            
            // get the inputs.  it will be an array of json objects
            var outputsArray = json["Outputs"] as JArray;

            // loop through the input array and determine the input state mask
            var outputsMask = 0;
            for (var i = 0; i < outputsArray.Count; i++)
            {
                var output = outputsArray[i];
                var state = (int)output["State"];
                if (state == 1)
                {
                    outputsMask |= (1 << i);
                }
            }

            TreeView.Invoke((MethodInvoker)delegate ()
            {
                _inputsStatusNode.Text = "Inputs - 0x" + inputsMask.ToString("X" + (inputCount / 4));
                _outputsStatusNode.Text = "Outputs - 0x" + outputsMask.ToString("X" + (outputCount / 4));
            });
        }

    }
}
