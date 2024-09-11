using System;
using System.Windows;
using System.Windows.Controls;
using Opc.UaFx.Server;

namespace OpcUaServerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpcServer _serverOpcUa = new OpcServer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void _serverOpcUa_Stopped(object sender, EventArgs e)
        {
            Includes.FormUtility.SetControl(this.StartServerButton, "Start");
            Includes.FormUtility.SetTextBlock(this.LogTextBlock, "Server connection stopped correctly.");
            this.EndpointTextBox.IsEnabled = true;
        }

        private void _serverOpcUa_Started(object sender, EventArgs e)
        {
            Includes.FormUtility.SetControl(this.StartServerButton, "Terminate");
            Includes.FormUtility.SetTextBlock(this.LogTextBlock, "Server connection started correctly.");
            this.EndpointTextBox.IsEnabled = false;
        }

        private void StartServerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_serverOpcUa.State == Opc.UaFx.Server.OpcServerState.Started || _serverOpcUa.State == Opc.UaFx.Server.OpcServerState.Starting)
                {
                    _serverOpcUa.Stop();
                }
                else
                {
                    string endpoint = this.EndpointTextBox.Text;

                    #region Create a custom Address Space with a Root Node for the Default Namespace http://{host}/{path}/nodes/:

                    //var machineNode = new OpcFolderNode("Machine");
                    //var machineIsRunningNode = new OpcDataVariableNode<bool>(machineNode, "IsRunning");

                    //_serverOpcUa = new OpcServer(endpoint, machineNode);

                    #endregion

                    #region Introduce a custom Node-Manager to the Server

                    _serverOpcUa = new OpcServer(endpoint, new Includes.OpcUa.NodeManagerA3());

                    #endregion

                    _serverOpcUa.Started += _serverOpcUa_Started;
                    _serverOpcUa.Stopped += _serverOpcUa_Stopped;
                    _serverOpcUa.Start();
                }

            }
            catch (System.Net.Sockets.SocketException ex) { Includes.FormUtility.SetTextBlock(this.LogTextBlock, $"Try to change the port.\r\nMessage {ex.Message}", false); }
            catch (Exception ex) { Includes.FormUtility.SetTextBlock(this.LogTextBlock, ex.Message, false); }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }
    }
}
