using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TurneroCustomControlLibrary;

namespace TurneroPrincipal
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItemSalir_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuItemAtender_Click(object sender, RoutedEventArgs e)
        {
            Process objProcess = new Process();
            objProcess.StartInfo.FileName = "D:/Projects GitHub/Turnero/Turnero/TurneroViewer/TurneroAtencion2/bin/Debug/TurneroAtencion2.exe";
            objProcess.Start();
        }

        private void MenuItemRegistrar_Click(object sender, RoutedEventArgs e)
        {
            Process objProcess = new Process();
            objProcess.StartInfo.FileName = "D:/Projects GitHub/Turnero/Turnero/TurneroViewer/TurneroRegistrador/bin/Debug/TurneroRegistrador.exe";
            objProcess.StartInfo.Arguments = "";
            objProcess.Start();
        }

        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {
            Help.ShowHelp(null, "HelpStudioSample.chm", HelpNavigator.Topic, null);
        }
    }
}
