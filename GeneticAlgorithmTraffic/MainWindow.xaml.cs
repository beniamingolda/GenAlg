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
            setup.ShowCity( MyMapControl,Variables.SIMULATION_TIME);
            setup.GenerateCars(100);
        }


        /*private void StartSimulation_Click(object sender, RoutedEventArgs e)
        {
            setup.Start(MyMapControl);
        }*/

        private void Sim1_Click(object sender, RoutedEventArgs e)
        {
            
            setup.LoadNextSimulation(Variables.SIMULATION_TIME);
            setup.Start(MyMapControl);
            setup.LoadNextSimulation(Variables.SIMULATION_TIME * 2);
            setup.Start(MyMapControl);
            setup.LoadNextSimulation(Variables.SIMULATION_TIME * 3);
            setup.Start(MyMapControl);
            setup.LoadNextSimulation(Variables.SIMULATION_TIME * 1.5);
            setup.Start(MyMapControl);
        }

        
        private void Algorytm_Genetyczny_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < 10; i++)
            {
                setup.Algorithm();
                setup.AlgorithmSim(MyMapControl);
            }
            
        }


    }
}
