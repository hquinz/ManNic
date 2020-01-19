using System.Windows;
using HQ4P.Tools.ManNic.NicManagement;
using HQ4P.Tools.ManNic.View.Models;




namespace HQ4P.Tools.ManNic.View
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NicCollector _nicCollector = new NicCollector();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new VmMainMaster(_nicCollector);
        }
    }
}
