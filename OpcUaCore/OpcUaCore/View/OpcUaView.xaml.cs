using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpcUaCore.View
{
    /// <summary>
    /// Interaction logic for OpcUaView.xaml
    /// </summary>
    public partial class OpcUaView : Window
    {
        private ViewModel.OpcUaViewModel _opcUaVM = new ViewModel.OpcUaViewModel();

        public OpcUaView()
        {
            InitializeComponent();
            this.DataContext = _opcUaVM;
        }
    }
}
