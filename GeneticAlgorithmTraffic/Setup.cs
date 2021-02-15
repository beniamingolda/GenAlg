using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI.Wpf;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GeneticAlgorithmTraffic
{
    class Setup
    {
        public List<Car> cars;
        public List<Crossing> crossings;
        public List<Node> trafficLights;

        public static TextBox textBox;


        public LoadMapNodes loadMapNodes;

        private Point mapMinPoint;
        private Point mapMaxPoint;

        private static List<double[]> carsMapPoints;
        private static List<double[]> redLightsPoints;
        private static List<double[]> greenLightsPoints;


        private static MemoryLayer carsLayer;
        private static MemoryLayer redLightsLayer;
        private static MemoryLayer greenLightsLayer;


        public static long time=0;


        public List<long> genSum;
        public List<double[]> genChangeTime;
        public List<double[]> resultOfAlgorithm;

        public bool firstSim = true;
        public long bestSum;
        public double[] bestChangeTime;
        int pokolenie = 0;
        public Setup(TextBox Logs)
        {
            textBox = Logs;
            PrintLog("Wczytywanie");
            cars = new List<Car>();
            crossings = new List<Crossing>();
            trafficLights = new List<Node>();
            genSum = new List<long>();
            genChangeTime = new List<double[]>();

            new Task(() =>
            {
                loadMapNodes = new LoadMapNodes();
                PrintLog("Wczytano");
            }).Start();
            
        }

        public void PrintLog(string text)
        {
            textBox.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                textBox.AppendText("[" + DateTime.Now.ToString("hh:mm:ss") + "]: " + text + "\n");
                textBox.ScrollToEnd();
            }));
        }


        public void ShowCity( MapControl mapControl,int croTime)
        {

            //wczytywanie granic miasta
            PrintLog("Wczytywanie granic");
            Thread bordersThread = new Thread(new ParameterizedThreadStart(LoadBorders));
            //wczytywanie sygnalizaji świetlnej 
            //skrzyżowań
            Thread crossingsThread = new Thread(new ParameterizedThreadStart(LoadCrossings));

            
            bordersThread.Start("Kielce");
            bordersThread.Join();
            PrintLog("Granice wczytano");
            crossingsThread.Start(croTime);
            
            mapControl.Navigator.NavigateTo(new BoundingBox(mapMinPoint, mapMaxPoint));
        }

        public void LoadBorders(object cityName)
        {
           

            var bundaryNodes = loadMapNodes.LoadCity((string)cityName);

            var minLon = 900.0;
            var maxLon = -900.0;
            var minLat = 900.0;
            var maxLat = -900.0;
            foreach (Node n in bundaryNodes)
            {
                if (!LoadMapNodes.allNodes.ContainsKey((long)n.Id)) 
                    LoadMapNodes.allNodes.Add((long)n.Id, n);
                if (n.Latitude < minLat) minLat = (double)n.Latitude;
                if (n.Longitude < minLon) minLon = (double)n.Longitude;
                if (n.Latitude > maxLat) maxLat = (double)n.Latitude;
                if (n.Longitude > maxLon) maxLon = (double)n.Longitude;
            }
            mapMinPoint = SphericalMercator.FromLonLat(minLon, minLat);
            mapMaxPoint = SphericalMercator.FromLonLat(maxLon, maxLat);
            loadMapNodes.LoadBoundaryPoints(new Point(maxLon, maxLat), new Point(minLon, minLat));
            
        }

        public void LoadCrossings(object croTime)
        {
            PrintLog("Wczytywanie sygnalizacji");
            trafficLights = loadMapNodes.LoadTrafficLights();
            var tmpTrafficLights = trafficLights;

            var counter = 0;
            while (tmpTrafficLights.Count > 0)
            {
                var crossing = new Crossing(counter++);
                tmpTrafficLights = crossing.SetupTrafficLights(tmpTrafficLights, (int)croTime);
                crossings.Add(crossing);
                
            }



        }


        public void GenerateCars(int numberOfCars)
        {
            
           

            for(var i = 0; i < numberOfCars; i++)
             {
                 Thread singleCar = new Thread(() =>
                 {
                     var c = new Car(i + 1, loadMapNodes.LoadCityNodes());

                     cars.Add(c);

                     PrintLog("Pojazd " + i.ToString() + "  dodany");
                 });

                 singleCar.Start();
                 singleCar.Join();

             }



         }

         private static void CarsLayerUpdate(MapControl myMapControl)
         {
             if (carsLayer == null)
             {
                 var feature = new Feature
                 {
                     Geometry = new MultiPoint(carsMapPoints)
                 };
                 var provider = new MemoryProvider(feature);
                 carsLayer = new MemoryLayer
                 {
                     DataSource = provider,
                     Style = new SymbolStyle
                     {
                         Fill = new Brush(new Color(0, 0, 0)),
                         SymbolScale = 0.25
                     }
                 };
                 myMapControl.Map.Layers.Add(carsLayer);
             }
             else
             {
                 carsLayer.DataSource = new MemoryProvider(
                     new Feature
                     {
                         Geometry = new MultiPoint(carsMapPoints)
                     });
                 myMapControl.Refresh();
                 //carsLayer.ClearCache();
                 carsLayer.RefreshData(carsLayer.Envelope, 1, true);//.ViewChanged(true, carsLayer.Envelope, 1);
             }
         }

         private static void TrafficLightsLayerUpdate(MapControl myMapControl)
         {
             if (redLightsLayer == null)
             {
                 var feature = new Feature
                 {
                     Geometry = new MultiPoint(redLightsPoints)
                 };
                 var provider = new MemoryProvider(feature);
                 redLightsLayer = new MemoryLayer { DataSource = provider, Style = new SymbolStyle { Fill = new Brush(Color.Red), SymbolScale = 0.33 } };
                 myMapControl.Map.Layers.Add(redLightsLayer);
             }
             else
             {
                 redLightsLayer.DataSource = new MemoryProvider(new Feature { Geometry = new MultiPoint(redLightsPoints) });
                 myMapControl.Refresh();
                 //redTrafficSignalsLayer.		ClearCache();
                 redLightsLayer.RefreshData(redLightsLayer.Envelope, 1, true); //ViewChanged(true, redTrafficSignalsLayer.Envelope, 1);
             }

             if (greenLightsLayer == null)
             {
                 var feature = new Feature
                 {
                     Geometry = new MultiPoint(greenLightsPoints)
                 };
                 var provider = new MemoryProvider(feature);
                 greenLightsLayer = new MemoryLayer { DataSource = provider, Style = new SymbolStyle { Fill = new Brush(Color.Green), SymbolScale = 0.33 } };
                 myMapControl.Map.Layers.Add(greenLightsLayer);
             }
             else
             {
                 greenLightsLayer.DataSource = new MemoryProvider(new Feature { Geometry = new MultiPoint(greenLightsPoints) });
                 myMapControl.Refresh();
                 //greenTrafficSignalsLayer.ClearCache();
                 greenLightsLayer.RefreshData(greenLightsLayer.Envelope, 1, true);//ViewChanged(true, greenTrafficSignalsLayer.Envelope, 1);
             }
         }
         public void LoadNextSimulation(double times)
         {
             foreach(var c in cars)
             {
                 c.ResetPositions();
             }
             foreach(var cro in crossings)
             {
                 cro.ResetAndChangeTime(times);
             }
             time =0;


         }
         public void Start(MapControl myMapControl)
         {
             Task simulation=new Task(()=> { SimulationStart(myMapControl); }) ;
             simulation.Start();
             simulation.Wait();
         }

        public void Start2(MapControl myMapControl)
        {
            Task simLoop = new Task(() =>
              {
                  SimLoop(myMapControl);
              });
            simLoop.Start();
        }

        public void SimLoop(MapControl myMapControl)
        {
            for( int i = 0; i < 4; i++)
            {
                LoadNextSimulation(Variables.SIMULATION_TIME*(i+1));
                SimulationStart(myMapControl);
            }
        }

         public void SimulationStart(MapControl myMapControl)
         {

             greenLightsPoints = new List<double[]>();
             redLightsPoints = new List<double[]>();
             carsMapPoints = new List<double[]>();
             var redLights = new List<TrafficLight>();
             var updatedTL = true;
             var updatedC = true;

             foreach(var cro in crossings)
             {
                 foreach(var tl in cro.trafficLights)
                 {
                     if (tl.greenOn == true)
                     {
                         greenLightsPoints.Add(SphericalMercator.FromLonLat((double)tl.trafficLightNode.Longitude, (double)tl.trafficLightNode.Latitude).ToDoubleArray());
                     }
                     else
                     {
                         redLights.Add(tl);
                         redLightsPoints.Add(SphericalMercator.FromLonLat((double)tl.trafficLightNode.Longitude, (double)tl.trafficLightNode.Latitude).ToDoubleArray());
                     }
                 }
             }
             foreach (var c in cars)
             {
                 carsMapPoints.Add(SphericalMercator.FromLonLat(c.position.X, c.position.Y).ToDoubleArray());
             }

             var loop = true;
             //dopoki samochody jeżdżą to działa pętla
             PrintLog("Start symulacji");
             while (loop)
             {
                 time++;
                 foreach(var cro in crossings)
                 {
                     if (time % cro.changeTime == 0)
                     {

                         updatedTL = true;
                         cro.ChangeLights();
                     }
                 }
                 if (updatedTL)
                 {
                     greenLightsPoints = new List<double[]>();
                     redLightsPoints = new List<double[]>();
                     redLights = new List<TrafficLight>();
                     foreach (var cro in crossings)
                     {
                         foreach(var tl in cro.trafficLights)
                         {
                             if (tl.greenOn == true)
                             {

                                 greenLightsPoints.Add(SphericalMercator.FromLonLat((double)tl.trafficLightNode.Longitude, (double)tl.trafficLightNode.Latitude).ToDoubleArray());
                             }
                             else
                             {
                                 redLights.Add(tl);
                                 redLightsPoints.Add(SphericalMercator.FromLonLat((double)tl.trafficLightNode.Longitude, (double)tl.trafficLightNode.Latitude).ToDoubleArray());
                             }
                         }
                     }
                     TrafficLightsLayerUpdate(myMapControl);
                     updatedTL = false;
                 }
                 foreach(var c in cars)
                 {
                     if (time % c.nextUpdate == 0)
                     {
                         updatedC = true;
                         c.Update(redLights);
                     }
                 }
                 if (updatedC)
                 {
                     carsMapPoints = new List<double[]>();
                     foreach(var c in cars)
                     {
                         carsMapPoints.Add(SphericalMercator.FromLonLat(c.position.X, c.position.Y).ToDoubleArray());
                     }
                     CarsLayerUpdate(myMapControl);
                     updatedC = false;
                 }
                 var test = 0;
                 foreach(var c in cars)
                 {

                     if (c.finish == false)
                     {

                         test = test + 1;
                     }
                 }
                 if (test == 0)
                 {
                     long sum=0;
                     loop = false;
                     foreach(var c in cars)
                     {
                         sum = sum + c.waitingTime;
                     }
                     PrintLog("Czas postoju na czerwonym " + sum);
                     genSum.Add(sum);
                     double[] listOfTimes=new double[crossings.Count];
                     var i = 0;
                     foreach(var cro in crossings)
                     {
                         listOfTimes[i] = cro.changeTime;
                         i++;
                     }                   
                     genChangeTime.Add(listOfTimes);                    
                 }
             }
         }


         public void Algorithm()
         {
             //ocena
             long sumOfTimes = 0;
             foreach(var s in genSum)
             {
                 sumOfTimes += s;
             }
             double[] ocena = new double[genSum.Count];
             double ocenaSum = 0;
             for(int i=0; i < ocena.Count(); i++)
             {
                 ocena[i] = sumOfTimes / (double)genSum[i];
                 ocenaSum = ocenaSum + ocena[i];
             }
             double[] ocena2 = new double[ocena.Count()];
             for (int i = 0; i < ocena2.Count(); i++)
             {
                 ocena2[i] = ocena[i] / ocenaSum;
             }
             var tmpBest = genSum[0];
             var tmpChangeTime = new double[genChangeTime.Count()];
             for(var o=0;o<ocena2.Count();o++)
             {
                 if (tmpBest > genSum[o])
                 {
                     tmpBest = genSum[o];
                     tmpChangeTime = genChangeTime[o];
                 }
             }
             //zapisanie najlepszego rozwiązania
             if (firstSim==true)
             {
                 bestSum = tmpBest;
                 bestChangeTime = tmpChangeTime;
                 firstSim = false;
             }
             else
             {
                 if (bestSum > tmpBest)
                 {
                     bestSum = tmpBest;
                     bestChangeTime = tmpChangeTime;
                 }
             }
             PrintLog("Best time=" + bestSum);

             //reprodukcja
             List<double[]> reprodukcja = new List<double[]>();
             var x = 0;
             var z = 0;
             var loop = true;
             var d = Variables.random.NextDouble();
             while (loop)
             {
                 if (d<=ocena2[x])
                 {
                     reprodukcja.Add( genChangeTime[x]);
                     x = 0;
                     z++;
                     d = Variables.random.NextDouble();
                     if (z == genChangeTime.Count()) 
                     {
                         loop = false;
                     }

                 }
                 else
                 {
                     d -= ocena2[x];
                     x++;
                 }

             }
             /*foreach(var r in reprodukcja)
             {
                 PrintLog("reprodukowane= " + r[0]);
             }
             */

            //krzyżowanie
            List<double[]> krzyzowanie = new List<double[]>();

            for(int i = 0; i < reprodukcja.Count(); i++)
            {            
                var k1 = Variables.random.Next() % reprodukcja.Count();
                var k2 = Variables.random.Next() % reprodukcja.Count();
                double[] tab = new double[reprodukcja[i].Count()];
                var l = Variables.random.Next() % reprodukcja[i].Count();
                for(var j=0; j < reprodukcja[i].Count(); j++)
                {
                    if (j <= l)
                    {
                        tab[j]=reprodukcja[k1][j];
                    }
                    else
                    {
                        tab[j] = reprodukcja[k2][j];
                    }
                }
                krzyzowanie.Add(tab);

            }
            /*foreach( var k in krzyzowanie)
            {
                PrintLog("kolekcja");
                foreach(var r in k)
                {
                    PrintLog("krzyzowanie= " + r);
                }
            }*/
            


            //mutacja
            foreach(var k in krzyzowanie)
            {
                if ((Variables.random.Next() % 100) <= 20)
                {
                    var mutPos = Variables.random.Next() % k.Count();
                    k[mutPos] = 10000+Variables.random.Next() % 20000;
                }
                
            }
            resultOfAlgorithm = krzyzowanie;
            
            
            genChangeTime = new List<double[]>();
            genSum = new List<long>();
            //wczytać dane do symulacji
            //dane dla pierwszej listy crossing
            
        }

        public void AlgorithmSim( MapControl myMapControl)
        {
            pokolenie++;
            PrintLog("Pokolenie " + pokolenie);
            foreach(var r in resultOfAlgorithm)
            {
                var i = 0;
                foreach(var cr in r)
                {
                    crossings[i].ResetAndChangeTime(cr);
                    i++;
                }
                foreach (var c in cars)
                {
                    c.ResetPositions();
                }
                time = 0;

                //start symulacji
                SimulationStart(myMapControl);
            }
            
        }

    }
}
