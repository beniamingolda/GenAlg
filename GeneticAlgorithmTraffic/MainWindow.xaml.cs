using BruTile.Predefined;
using Mapsui.Layers;
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
        public MainWindow()
        {
            InitializeComponent();

            MyMapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()));
            setup = new Setup(Logs);
            
        }

        private void ShowCity_Click(object sender, RoutedEventArgs e)
        {
            setup.ShowCity("Kielce", MyMapControl);
        }

        private void AddCars_Click(object sender, RoutedEventArgs e)
        {
            //setup.PrintLog("TEST");
            setup.GenerateCars(10);
        }

        private void StartSimulation_Click(object sender, RoutedEventArgs e)
        {
            setup.SimulationStart(MyMapControl);
        }
    }
}
