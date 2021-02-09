﻿using Mapsui.Geometries;
using Mapsui.Projection;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmTraffic
{
    class Car
    {
        public int id;
        public Road road;
        public Point position;
        public Node positionNode;
        public long nextUpdate = 10;
        public bool stoppedOnLight = false;
        public bool finish = false;
        public Car(int id, List<Node> placeList)
        {
            this.id = id;
            var tmpPos = Variables.random.Next() % placeList.Count();
            var startPosition = new Point((double)placeList[tmpPos].Longitude, (double)placeList[tmpPos].Latitude);
            tmpPos = Variables.random.Next() % placeList.Count();
            var endPosition= new Point((double)placeList[tmpPos].Longitude, (double)placeList[tmpPos].Latitude);
            position = startPosition;
            road = new Road(startPosition, endPosition);
            
        }
        public void Update(List<TrafficLight> redLights)
        {
            Node next=new Node();
            //jeśli wywołana to jedzie
            //jeśli stoppedOnLight to stoi
            //jeśli finisz to stoi
            if (finish == false)
            {

                //if next node czerwone to stoi
                //nextUpdate wywołuje się za tyle ile światło sie zmieni
                if (stoppedOnLight == true)
                {
                    stoppedOnLight = false;
                    nextUpdate = (long)(Variables.DistanceInKmBetweenEarthCoordinates((double)next.Latitude, (double)next.Longitude, position.Y, position.X) * 1000 / 13.89 * 1000);
                    //nowa pozycja
                    position.X = (double)next.Longitude;
                    position.Y = (double)next.Latitude;
                    //sprawdzić czy nadal czerwone
                    //nextUpdate=
                }
                else if (stoppedOnLight == false)
                {


                    //następna pozycja
                    next = road.NextNode();
                    var nextPos=SphericalMercator.FromLonLat((double)next.Longitude, (double)next.Latitude).ToDoubleArray();
                    
                    //czy czerwone
                    foreach (var red in redLights)
                    {
                        //test czy czerwone
                        if (nextPos == SphericalMercator.FromLonLat((double)red.trafficLightNode.Longitude, (double)red.trafficLightNode.Latitude).ToDoubleArray())
                        {
                            stoppedOnLight = true;
                            nextUpdate = (long)red.lightChangeTime;
                        }
                    }
                    if (stoppedOnLight == false)
                    {
                        //czas przejazdu
                        nextUpdate = (long)(Variables.DistanceInKmBetweenEarthCoordinates((double)next.Latitude, (double)next.Longitude, position.Y, position.X) * 1000 / 13.89 * 1000);
                        //nowa pozycja
                        position.X = (double)next.Longitude;
                        position.Y = (double)next.Latitude;
                    }
                    /*if(stoppedOnLight==true)
                    {
                        //nextUpdate=czas zmiany na zielone
                    }*/
                    

                }
                if (road.over == true)
                {
                    finish = true;
                }
            }
        }
    }
}