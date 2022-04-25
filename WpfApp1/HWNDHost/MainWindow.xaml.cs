using Microsoft.VisualStudio.Utilities;
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

namespace HWNDHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HwndHostControl hostControl;
        public MainWindow()
        {
            InitializeComponent();
           // var button = this.CreateButton();
            var hostedui = new HostedButton()
            {
                DataContext = this.DataContext
            };
            using (DpiAwareness.EnterDpiScope(DpiAwareness.ProcessDpiAwarenessContext))
            {
                
                var hwndSource = new HwndSourceAdapter(hostedui);
                this.hostControl = new HwndHostControl(hwndSource);
            }
           
            this.HwndhostControl.Children.Add(hostControl);
        }

        
    }
}
