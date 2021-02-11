using BruTile.Predefined;
using Mapsui.Layers;
using Mapsui.UI.Wpf;
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

namespace GeneticAlgorithmTraffic
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Setup setup;
        MapControl backupMap;
        public MainWindow()
        {
            InitializeComponent();

            MyMapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()));
            setup = new Setup(Logs);
            backupMap = MyMapControl;
            
        }

        private void ShowCity_Click(object sender, RoutedEventArgs e)
        {
            setup.ShowCity("Kielce", MyMapControl,Variables.MIN_IN_MILIS);
            setup.GenerateCars(1);
        }

        private void AddCars_Click(object sender, RoutedEventArgs e)
        {
            //setup.PrintLog("TEST");
            setup.GenerateCars(10);
        }

        private void StartSimulation_Click(object sender, RoutedEventArgs e)
        {
            setup.Start(MyMapControl);
        }

        private void Sim1_Click(object sender, RoutedEventArgs e)
        {
            MyMapControl = backupMap;
            setup.LoadNextSimulation(Variables.MIN_IN_MILIS);
        }

        private void Sim2_Click(object sender, RoutedEventArgs e)
        {
            MyMapControl = backupMap;
            setup.LoadNextSimulation(Variables.MIN_IN_MILIS * 2);
        }

        private void Sim3_Click(object sender, RoutedEventArgs e)
        {
            MyMapControl = backupMap;
            setup.LoadNextSimulation(Variables.MIN_IN_MILIS * 3);
        }

        private void Sim4_Click(object sender, RoutedEventArgs e)
        {
            MyMapControl = backupMap;
            setup.LoadNextSimulation(Variables.MIN_IN_MILIS * 1.5);
        }

        private void Algorytm_Genetyczny_Click(object sender, RoutedEventArgs e)
        {
            setup.Algorithm();
        }

        private void Symulacja_Gen_Click(object sender, RoutedEventArgs e)
        {
            setup.AlgorithmSim(0,MyMapControl);
        }
    }
}
