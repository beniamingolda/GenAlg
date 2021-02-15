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
        int mut;
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
            setup.GenerateCars(int.Parse(NumOfCars.Text));
        }


        /*private void StartSimulation_Click(object sender, RoutedEventArgs e)
        {
            setup.Start(MyMapControl);
        }*/

        private void Sim1_Click(object sender, RoutedEventArgs e)
        {
            setup.Start2(MyMapControl,int.Parse(RandStart.Text)*1000, int.Parse(RandEnd.Text)*1000,4);
            mut = int.Parse(MutationPercent.Text);
            /*setup.LoadNextSimulation(Variables.SIMULATION_TIME);
            setup.Start(MyMapControl);
            setup.LoadNextSimulation(Variables.SIMULATION_TIME * 2);
            setup.Start(MyMapControl);
            setup.LoadNextSimulation(Variables.SIMULATION_TIME * 3);
            setup.Start(MyMapControl);
            setup.LoadNextSimulation(Variables.SIMULATION_TIME * 1.5);
            setup.Start(MyMapControl);*/
        }

        
        private void Algorytm_Genetyczny_Click(object sender, RoutedEventArgs e)
        {

            Task taskGen = new Task(() =>
            {
                TaskGen(MyMapControl, mut);
            });
            taskGen.Start();
            /*for(int i = 0; i < 10; i++)
            {
                setup.Algorithm();
                setup.AlgorithmSim(MyMapControl);
            }*/
            
        }
        public void TaskGen(MapControl myMapControl,int mutP)
        {
            for (int i = 0; i < 10; i++)
            {
                setup.Algorithm(mutP);
                setup.AlgorithmSim(MyMapControl);
            }
        }


    }
}
