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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TurneroGraficos
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModels.MainWindowModel viewModel;

        public MainWindow()
        {
            viewModel = new ViewModels.MainWindowModel();
            DataContext = viewModel;

            CompositionTarget.Rendering += CompositionTargetRendering;

            InitializeComponent();
        }

        private long frameCounter;
        private Stopwatch stopwatch = new Stopwatch();
        private long lastUpdateMilliSeconds;

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (stopwatch.ElapsedMilliseconds > lastUpdateMilliSeconds + 5000)
            {
                viewModel.UpdateModel();
                Plot1.InvalidatePlot(true);
                lastUpdateMilliSeconds = stopwatch.ElapsedMilliseconds;
            }
        }
    }


}
