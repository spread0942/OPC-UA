using Opc.UaFx;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpcUaServerWpf.Includes.OpcUa
{
    class NodeManagerA3 :
        Opc.UaFx.Server.OpcNodeManager
    {
        public NodeManagerA3()
        : base("http://mynamespace/")
        {
        }

        protected override IEnumerable<IOpcNode> CreateNodes(OpcNodeReferenceCollection references)
        {
            // Define custom root node.
            var machineNode = new OpcFolderNode(new OpcName("Machine", this.DefaultNamespaceIndex));

            // Add custom root node to the Objects-Folder (the root of all server nodes):
            references.Add(machineNode, OpcObjectTypes.ObjectsFolder);

            // Add custom sub node beneath of the custom root node:
            var isMachineRunningNode = new OpcDataVariableNode<bool>(machineNode, "IsRunning");

            // Return each custom root node using yield return.
            yield return machineNode;
        }
    }
}
