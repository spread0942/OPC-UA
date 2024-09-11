using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Opc.UaFx.Client;
using Opc.UaFx;
using System.ServiceModel.Channels;
using System.Windows;
using Microsoft.Extensions.Options;

namespace OpcUaCore.ViewModel
{
    class OpcUaViewModel :
        INotifyPropertyChanged
    {
        #region Fields
        private OpcClient _opcUaClient;
        OpcSubscription _opcSubscription;
        #endregion

        #region View Properties
        private string _endpoint;

        public string Endpoint
        {
            get { return _endpoint; }
            set
            {
                _endpoint = value;
                this.NotifyPropertyChanged();
            }
        }

        private ObservableCollection<Model.OpcUaType> _nodeTypes;

        public ObservableCollection<Model.OpcUaType> NodeTypes
        {
            get { return _nodeTypes; }
            private set { _nodeTypes = value; }
        }

        private Model.OpcUaType _selectedType;

        public Model.OpcUaType SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                this.NotifyPropertyChanged();
            }
        }

        private string _node;

        public string Node
        {
            get { return _node; }
            set
            {
                _node = value;
                this.NotifyPropertyChanged();
            }
        }

        private object _value;

        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                this.NotifyPropertyChanged();
            }
        }

        private object _subscribeValue;

        public object SubscribeValue
        {
            get { return _subscribeValue; }
            set
            {
                _subscribeValue = value;
                this.NotifyPropertyChanged();
            }
        }

        private object _buttonConnection;

        public object ButtonConnection
        {
            get { return _buttonConnection; }
            set
            {
                _buttonConnection = value;
                this.NotifyPropertyChanged();
            }
        }

        private System.Windows.Media.Brush _logForeground;

        public System.Windows.Media.Brush LogForeground
        {
            get { return _logForeground; }
            set
            {
                _logForeground = value;
                this.NotifyPropertyChanged();
            }
        }

        private object _log;

        public object Log
        {
            get { return _log; }
            set
            {
                _log = value;
                this.NotifyPropertyChanged();
            }
        }

        #endregion

        #region Commands

        private Command.CommandBase _connectCommand;

        public Command.CommandBase ConnectCommand
        {
            get { return _connectCommand; }
            set { _connectCommand = value; }
        }

        public void Execute()
        {
            try
            {
                System.Threading.Thread opcUaConnectionThread = new System.Threading.Thread(() =>
                {
                    try
                    {
                        if (_opcUaClient != null && _opcUaClient.State == OpcClientState.Connected)
                        {
                            _opcUaClient.Disconnect();
                        }
                        else
                        {
                            _opcUaClient = new OpcClient(this.Endpoint);
                            _opcUaClient.Connected += _opcUaClient_Connected;
                            _opcUaClient.Connecting += _opcUaClient_Connecting;
                            _opcUaClient.Disconnected += _opcUaClient_Disconnected;
                            _opcUaClient.Disconnecting += _opcUaClient_Disconnecting;

                            _opcUaClient.Connect();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Log = ex.Message;
                        this.LogForeground = System.Windows.Media.Brushes.Red;
                    }
                });
                opcUaConnectionThread.Start();
            }
            catch (Exception ex)
            {
                this.Log = ex.Message;
                this.LogForeground = System.Windows.Media.Brushes.Red;
            }
        }

        private Command.CommandBase _readCommand;

        public Command.CommandBase ReadCommand
        {
            get { return _readCommand; }
            set { _readCommand = value; }
        }

        public void ExecuteRead()
        {
            try
            {
                if (string.IsNullOrEmpty(this.Node))
                {
                    this.Log = "Node empty.";
                    this.LogForeground = System.Windows.Media.Brushes.Red;
                }
                else if (_opcUaClient == null || (_opcUaClient != null && _opcUaClient.State != OpcClientState.Connected))
                {
                    this.Log = "Can't read, machine not connected.";
                    this.LogForeground = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    dynamic valueNode = _opcUaClient.ReadNode(this.Node);
                    string valueRead = null;

                    //System.Windows.MessageBox.Show(valueNode.Value.GetType().ToString());

                    if (valueNode.Value is Array)
                    {
                        var arr = valueNode.Value as Array;

                        valueRead = $"({arr.Length})";

                        foreach (var el in arr)
                        {
                            valueRead += el.ToString() + "|";
                        }

                        valueRead = valueRead.Trim('|');
                    }
                    else
                    {
                        valueRead = valueNode.Value.ToString();
                    }

                    this.Value = valueRead;

                    OpcSubscribeDataChange[] commands = new OpcSubscribeDataChange[] {
                        new OpcSubscribeDataChange(this.Node, HandleEvemt)
                    };

                    if (_opcSubscription != null)
                        _opcSubscription.Unsubscribe();

                    _opcSubscription = _opcUaClient.SubscribeNodes(commands);

                    //if (valueNode.Value is UInt16[])
                    //{
                    //    if (this.SelectedType == Model.OpcUaType.ArrWord)
                    //    {
                    //        string convertedValue = string.Empty;
                    //        UInt16[] arrayValue = (UInt16[])valueNode.Value;

                    //        foreach (UInt16 element in arrayValue)
                    //        {
                    //            byte[] bytes = BitConverter.GetBytes(element);
                    //            foreach (var b in bytes)
                    //            {
                    //                convertedValue += (char)b;
                    //            }
                    //        }

                    //        valueRead = convertedValue;
                    //    }
                    //    else
                    //    {
                    //        this.Log = "Wrong type selected, the value is a ushort[] you must select ArrWord.";
                    //        this.LogForeground = System.Windows.Media.Brushes.Red;
                    //    }
                    //}
                    //else if (valueNode.Value is Int32[])
                    //{
                    //    if (this.SelectedType == Model.OpcUaType.ArrWord32)
                    //    {
                    //        string convertedValue = string.Empty;
                    //        Int32[] arrayValue = (Int32[])valueNode.Value;

                    //        foreach (Int32 element in arrayValue)
                    //        {
                    //            System.Windows.MessageBox.Show(element.ToString());
                    //            //byte[] bytes = BitConverter.GetBytes(element);
                    //            //foreach (var b in bytes)
                    //            //{
                    //            //    convertedValue += (char)b;
                    //            //}
                    //        }

                    //        valueRead = arrayValue.Length;
                    //    }
                    //    else
                    //    {
                    //        this.Log = "Wrong type selected, the value is a Int32[] you must select ArrWord.";
                    //        this.LogForeground = System.Windows.Media.Brushes.Red;
                    //    }
                    //}
                    //else
                    //{
                    //    valueRead = valueNode.Value.ToString();
                    //}

                    //this.Value = valueRead.ToString();
                }
            }
            catch (Exception ex)
            {
                this.Log = ex.Message;
                this.LogForeground = System.Windows.Media.Brushes.Red;
            }
        }

        public Command.CommandBase WriteCommand
        {
            get; set;
        }

        public void ExecuteWrite()
        {
            try
            {
                OpcStatus nodeStatus;

                if (this.SelectedType == Model.OpcUaType.Numeric && int.TryParse(this.Value.ToString(), out int numericCurrentContent))
                {
                    nodeStatus = _opcUaClient.WriteNode(this.Node, numericCurrentContent);
                }
                else if (this.SelectedType == Model.OpcUaType.Bool && bool.TryParse(this.Value.ToString(), out bool boolCurrentContent))
                {
                    nodeStatus = _opcUaClient.WriteNode(this.Node, boolCurrentContent);
                }
                else if (this.SelectedType == Model.OpcUaType.UInt16 && UInt16.TryParse(this.Value.ToString(), out UInt16 uInt16CurrentContent))
                {
                    nodeStatus = _opcUaClient.WriteNode(this.Node, uInt16CurrentContent);
                }

                else if (this.SelectedType == Model.OpcUaType.Byte && int.TryParse(this.Value.ToString(), out int byteVal))
                {
                    nodeStatus = _opcUaClient.WriteNode(this.Node, (byte)byteVal);
                }
                //sysSingle[]
                else if (this.SelectedType == Model.OpcUaType.arrSystemSingle)
                {
                    System.Single[] myarray;
                    string[] numeri = this.Value.ToString().Split('|');
                    myarray = new System.Single[numeri.Length];
                    int i = 0;
                    foreach (var el in numeri)
                    {
                        myarray[i] += System.Single.Parse(el);
                        i++;
                    }
                   
                    nodeStatus = _opcUaClient.WriteNode(this.Node, myarray);
                }
                
            
                else
                {
                    nodeStatus = _opcUaClient.WriteNode(this.Node, this.Value.ToString());
                    //MerlinShLib.PlugIn.OpcUa.OpcUaTask.StoreLog(sqlCore, endpoint, node, nodeStatus.Description);
                    //endpoint.UpdateCurrentContentLocalNode(sqlCore, node.Id);
                    //MerlinShLib.PlugIn.OpcUa.OpcUaTask.StoreLog(sqlCore, endpoint, node, "Pulizia delle memorie");
                }

             //   this.Log = nodeStatus.ToString();
            }
            catch (Exception ex)
            {
                this.Log = ex.Message;
                this.LogForeground = System.Windows.Media.Brushes.Red;
            }
        }

        #endregion

        #region OPC UA Events
        private void _opcUaClient_Disconnected(object sender, EventArgs e)
        {
            this.ButtonConnection = "Connect";
            this.Log = "Disconnect to the servier.";
            this.LogForeground = System.Windows.Media.Brushes.Green;
        }

        private void _opcUaClient_Connected(object sender, EventArgs e)
        {
            this.ButtonConnection = "Disconnect";
            this.Log = "Connect to the servier.";
            this.LogForeground = System.Windows.Media.Brushes.Green;
        }

        private void _opcUaClient_Disconnecting(object sender, EventArgs e)
        {
            this.Log = "Disconnecting to the servier...";
            this.LogForeground = System.Windows.Media.Brushes.Green;
        }

        private void _opcUaClient_Connecting(object sender, EventArgs e)
        {
            this.Log = "Connecting to the servier...";
            this.LogForeground = System.Windows.Media.Brushes.Green;
        }
        #endregion

        public OpcUaViewModel()
        {
            this.Endpoint = "opc.tcp://192.168.1.105:63840";
            this.Node = "ns=2;s=HMI_ProduzioneParziale";
            this.NodeTypes = new ObservableCollection<Model.OpcUaType>()
            {
                Model.OpcUaType.Numeric,
                Model.OpcUaType.Bool,
                Model.OpcUaType.String,
                Model.OpcUaType.ArrWord,
                Model.OpcUaType.ArrWord32,
                Model.OpcUaType.UInt16,
                Model.OpcUaType.Byte,
                Model.OpcUaType.arrSystemSingle
            };
            this.SelectedType = this.NodeTypes[0];
            this.ButtonConnection = "Connect";
            this.ConnectCommand = new Command.CommandBase(this.Execute);
            this.ReadCommand = new Command.CommandBase(this.ExecuteRead);
            this.WriteCommand = new Command.CommandBase(this.ExecuteWrite);
            this.Log = BitConverter.GetBytes(5);
        }

        //public ObservableCollection<string> Nodes { get; set; }

        ///// <summary>
        ///// Load all the OpcUa nodes
        ///// </summary>
        ///// <param name="node"></param>
        ///// <param name="treeItem"></param>
        //private void BrowseOpcUaServer(OpcNodeInfo node, ObservableCollection<string> treeItem)
        //{
        //    ObservableCollection<string> newItem = new ObservableCollection<string>();
        //    string displayName = (node.Attribute(OpcAttribute.DisplayName) == null) ? "NULL" : node.Attribute(OpcAttribute.DisplayName).Value.ToString();
        //    string dataType = node.NodeId.Type.ToString();
        //    string nodeId = node.NodeId.ToString();

        //    newItem.PreviewMouseDoubleClick += NewItem_PreviewMouseDoubleClick;
        //    newItem.Header = string.Format("Display name: ({0}); Data type: ({1}); Node id: ({2});",
        //            displayName,
        //            dataType,
        //            nodeId);
        //    newItem.Tag = node.NodeId;

        //    treeItem.Items.Add(newItem);

        //    foreach (var childNode in node.Children())
        //        BrowseOpcUaServer(childNode, newItem);
        //}

        private void HandleEvemt(object sender, OpcDataChangeReceivedEventArgs e)
        {
            this.SubscribeValue = e.Item.Value.ToString();
        }

        #region INorifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
