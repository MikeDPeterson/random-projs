using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFClock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer tmrMain;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click_1( object sender, RoutedEventArgs e )
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded_1( object sender, RoutedEventArgs e )
        {
            tmrMain = new Timer();
            tmrMain.Interval = 1000;
            tmrMain.Elapsed += tmrMain_Elapsed;
            tmrMain.Enabled = true;
        }

        void tmrMain_Elapsed( object sender, ElapsedEventArgs e )
        {
            Dispatcher.Invoke( () =>
            {
                lblClockHhMm.Content = DateTime.Now.ToString( "t" );
            } );
        }
    }
}
