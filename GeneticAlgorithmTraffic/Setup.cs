﻿using Mapsui.Geometries;
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
        //komunikaty
        public static TextBox textBox;

        public Simulation simulation;
        public OsmDataManager osmDataManager;

        private Point mapMinPoint;
        private Point mapMaxPoint;

        private static List<double[]> carsMapPoints;
        private static List<double[]> redLightsPoints;
        private static List<double[]> greenLightsPoints;


        private static MemoryLayer carsLayer;
        private static MemoryLayer redLightsLayer;
        private static MemoryLayer greenLightsLayer;


        public static long time=0;
        public Setup(TextBox Logs)
        {
            textBox = Logs;
            PrintLog("Wczytywanie");
            cars = new List<Car>();
            crossings = new List<Crossing>();
            trafficLights = new List<Node>();
            simulation = new Simulation();

            new Task(() =>
            {
                osmDataManager = new OsmDataManager();
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


        public void ShowCity(string cityName, MapControl mapControl)
        {

            //wczytywanie granic miasta
            PrintLog("Wczytywanie granic");
            Thread bordersThread = new Thread(new ParameterizedThreadStart(LoadBorders));
            //wczytywanie sygnalizaji świetlnej 
            //skrzyżowań
            Thread crossingsThread = new Thread(LoadTrafficLights);

            
            bordersThread.Start(cityName);
            bordersThread.Join();
            PrintLog("Granice wczytano");
            crossingsThread.Start();
            mapControl.Navigator.NavigateTo(new BoundingBox(mapMinPoint, mapMaxPoint));
        }

        public void LoadBorders(object cityName)
        {
           

            var bundaryNodes = osmDataManager.getCityBorderByNameFromRelation((string)cityName);

            var minLon = 900.0;
            var maxLon = -900.0;
            var minLat = 900.0;
            var maxLat = -900.0;
            foreach (Node n in bundaryNodes)
            {
                if (!OsmDataManager.allNodes.ContainsKey((long)n.Id)) OsmDataManager.allNodes.Add((long)n.Id, n);
                if (n.Latitude < minLat) minLat = (double)n.Latitude;
                if (n.Longitude < minLon) minLon = (double)n.Longitude;
                if (n.Latitude > maxLat) maxLat = (double)n.Latitude;
                if (n.Longitude > maxLon) maxLon = (double)n.Longitude;
            }
            mapMinPoint = SphericalMercator.FromLonLat(minLon, minLat);
            mapMaxPoint = SphericalMercator.FromLonLat(maxLon, maxLat);
            osmDataManager.SetBoundaryPoints(new Point(maxLon, maxLat), new Point(minLon, minLat));
            
        }

        public void LoadTrafficLights()
        {
            PrintLog("Wczytywanie sygnalizacji");
            trafficLights = osmDataManager.GetTrafficLights();
            var tmpTrafficLights = trafficLights;

            var counter = 0;
            while (tmpTrafficLights.Count > 0)
            {
                var crossing = new Crossing(counter++);
                tmpTrafficLights = crossing.SetupTrafficLights(tmpTrafficLights,Variables.MIN_IN_MILIS);
                crossings.Add(crossing);
            }
            PrintLog("Wczytano sygnalizację");
        }

        public void GenerateCars(int numberOfCars)
        {
            
            PrintLog("Generowanie pojazdów");
            for(var i = 0; i < numberOfCars; i++)
            {
                Thread singleCar = new Thread(() =>
                {
                    var c = new Car(i + 1, osmDataManager.GetCityNodes());
                    cars.Add(c);
                    PrintLog("pojazd " + i.ToString() + "  dodany");
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
                        Fill = new Brush(new Color(0, 0, 255)),
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
                //myMapControl.Refresh();
                //carsLayer.ClearCache();
                carsLayer.RefreshData(carsLayer.Envelope, 1, true);//.ViewChanged(true, carsLayer.Envelope, 1);
            }
        }

        private static void TrafficSignalsLayerUpdate(MapControl myMapControl)
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
                //myMapControl.Refresh();
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
                //myMapControl.Refresh();
                //greenTrafficSignalsLayer.ClearCache();
                greenLightsLayer.RefreshData(greenLightsLayer.Envelope, 1, true);//ViewChanged(true, greenTrafficSignalsLayer.Envelope, 1);
            }
        }

        public void SimulationStart(MapControl myMapControl)
        {
            
            greenLightsPoints = new List<double[]>();
            redLightsPoints = new List<double[]>();
            carsMapPoints = new List<double[]>();
            var redLights = new List<TrafficLight>();
            var updatedTL = false;
            var updatedC = false;
            //ustawić początkowe światła- niepowinno być problemu ponieważ pierwszy update samochodu jest po światłach ale lepiej ustawić
            //pętla 
            var loop = true;
            //dopoki samochody jeżdżą to działa pętla
            PrintLog("Start pętli");
            while (loop)
            {
                time++;
                PrintLog(time.ToString());
                foreach(var cro in crossings)
                {
                    if (time % cro.changeTime == 0)
                    {
                        updatedTL = true;
                        cro.ChangeLights();
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
                        //zmiana świateł
                    }
                }

                foreach(var c in cars)
                {
                    if (time % c.nextUpdate == 0)
                    {
                        updatedC = true;
                        //ruch samochodu
                        c.Update(redLights);
                        carsMapPoints.Add(SphericalMercator.FromLonLat(c.position.X, c.position.Y).ToDoubleArray());
                    }
                }

                //wyświetlanie
                if (updatedTL || updatedC)
                {
                    if (updatedTL)
                    {
                        TrafficSignalsLayerUpdate(myMapControl);
                    }
                    if (updatedC)
                    {
                        CarsLayerUpdate(myMapControl);
                    }

                    greenLightsPoints = new List<double[]>();
                    redLightsPoints = new List<double[]>();
                    carsMapPoints = new List<double[]>();
                    redLights = new List<TrafficLight>();
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
                    loop = false;
                }
            }
        }

    }
}