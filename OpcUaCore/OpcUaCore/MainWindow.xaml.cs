using Opc.UaFx;
using Opc.UaFx.Client;
using Opc.UaFx.Client.Classic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpcUaCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpcClient _opcClient = new OpcClient();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Browse(OpcNodeInfo node, TreeViewItem treeItem)
        {
            TreeViewItem newItem = new TreeViewItem();
            newItem.Header = string.Format("{0}({1}) type: ({2})",
                    node.Attribute(OpcAttribute.DisplayName).Value,
                    node.NodeId,
                    node.NodeId.Type);

            treeItem.Items.Add(newItem);

            foreach (var childNode in node.Children())
                this.Browse(childNode, newItem);
        }

        private void ConnectEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if (_opcClient.State == OpcClientState.Connected)
            {
                _opcClient.Disconnect();
            }
            else
            {
                MessageBox.Show("Provo a connettermi.");
                Uri endpointUri = new Uri(this.EndpointTextBlock.Text);
                _opcClient.ServerAddress = endpointUri;
                _opcClient.OperationTimeout = 7200000;
                _opcClient.SessionTimeout = 7200000;
                _opcClient.SessionName = "Merlin Connect OPC UA";
                //_opcClient.UseDynamic = true;
                _opcClient.Connected += _opcClient_Connected;
                _opcClient.Connecting += _opcClient_Connecting;
                MessageBox.Show($"Prima di connettermi con sessione {_opcClient.SessionName}.");
                _opcClient.Connect();
                
            }
        }

        private void _opcClient_Connecting(object sender, EventArgs e)
        {
            MessageBox.Show("Sto provando a connettermi....");
        }

        private void _opcClient_Connected(object sender, EventArgs e)
        {
            var node = _opcClient.BrowseNode(OpcObjectTypes.ObjectsFolder);
            this.Browse(node, this.RootTreeViewItem);
        }

        private void SendValueEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if (_opcClient.State == OpcClientState.Connected)
            {
                OpcStatus result;
                string v = this.NodoValueTextBlock.Text;
                
                result = _opcClient.WriteNode(this.NodoTextBlock.Text, OpcAttribute.Value, Convert.ToUInt16(v));

                MessageBox.Show(result.Description);
            }
            else
            {
                MessageBox.Show("Not connected");
            }
        }

        private void ReadValueEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if (_opcClient.State == OpcClientState.Connected)
            {
                string v = this.NodoValueTextBlock.Text;

                OpcValue result = _opcClient.ReadNode(this.NodoTextBlock.Text);

                MessageBox.Show(result.ToString());
            }
            else
            {
                MessageBox.Show("Not connected");
            }
        }

        private void DiscoverEndpoint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var discoveryClient = new OpcClassicDiscoveryClient(this.EndpointTextBlock.Text))
                {
                    var servers = discoveryClient.DiscoverServers();

                    foreach (var server in servers)
                    {
                        MessageBox.Show(string.Format(
                                 "- {0}, ClassId={1}, ProgId={2}",
                                 server.Name,
                                 server.ClassId,
                                 server.ProgId));
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            
        }
    }
}
