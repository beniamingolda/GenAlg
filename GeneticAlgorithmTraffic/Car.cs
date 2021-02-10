using Mapsui.Geometries;
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
        public long waitingTime=0;
        public long nextUpdate = 10;
        public bool stoppedOnLight = false;
        public bool finish = false;
        Node next=new Node();

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

        public void ResetPositions()
        {
            road.AddtoStack();
            road.over = false;
            finish = false;
            position = road.start;
            waitingTime = 0;
            nextUpdate = 10;
            stoppedOnLight = false;
            next = new Node();
        }

        
        public void Update(List<TrafficLight> redLights)
        {
            
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
                    if (nextUpdate == 0)
                    {
                        nextUpdate = 1;
                    }
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
                        var distance= Variables.DistanceInKmBetweenEarthCoordinates((double)next.Latitude, (double)next.Longitude, (double)red.trafficLightNode.Latitude, (double)red.trafficLightNode.Longitude) * 1000;
                        var redPos = SphericalMercator.FromLonLat((double)red.trafficLightNode.Longitude, (double)red.trafficLightNode.Latitude).ToDoubleArray();
                        //test czy czerwone
                        //if (nextPos == redPos)
                        //if(next==red.trafficLightNode)
                        if(distance<10)
                        {
                            stoppedOnLight = true;
                            waitingTime = waitingTime + ((long)red.lightChangeTime - nextUpdate);
                            nextUpdate = (long)red.lightChangeTime;
                            if (nextUpdate == 0)
                            {
                                nextUpdate = 1;
                            }
                        }
                    }
                    if (stoppedOnLight == false)
                    {
                        //czas przejazdu
                        nextUpdate = (long)(Variables.DistanceInKmBetweenEarthCoordinates((double)next.Latitude, (double)next.Longitude, position.Y, position.X) * 1000 / 13.89 * 1000);
                        if (nextUpdate == 0)
                        {
                            nextUpdate = 1;
                        }
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
