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
            if (finish == false)
            {
                if (stoppedOnLight == true)
                {
                    stoppedOnLight = false;
                    nextUpdate = (long)(Variables.DistanceInKm((double)next.Latitude, (double)next.Longitude, position.Y, position.X) * 1000 / 13.89 * 1000);

                    if (nextUpdate == 0)
                    {
                        nextUpdate = 1;
                    }
                    position.X = (double)next.Longitude;
                    position.Y = (double)next.Latitude;
                }
                else if (stoppedOnLight == false)
                {
                    next = road.NextNode();
                    foreach (var red in redLights)
                    {
                        var distance= Variables.DistanceInKm((double)next.Latitude, (double)next.Longitude, (double)red.trafficLightNode.Latitude, (double)red.trafficLightNode.Longitude) * 1000;
                        if(distance<10)
                        {
                            stoppedOnLight = true;
                            if(((long)red.lightChangeTime - nextUpdate) >= 0)
                            {
                                waitingTime = waitingTime + ((long)red.lightChangeTime - nextUpdate);
                            }
                            else
                            {
                                waitingTime = waitingTime + (long)red.lightChangeTime;
                            }
                            if (waitingTime < 0)
                            {
                                Console.WriteLine(waitingTime);
                            }
                            nextUpdate = (long)red.lightChangeTime;
                            if (nextUpdate == 0)
                            {
                                nextUpdate = 1;
                            }
                        }
                    }
                    if (stoppedOnLight == false)
                    {
                        nextUpdate = (long)(Variables.DistanceInKm((double)next.Latitude, (double)next.Longitude, position.Y, position.X) * 1000 / 13.89 * 1000);
                        if (nextUpdate == 0)
                        {
                            nextUpdate = 1;
                        }
                        position.X = (double)next.Longitude;
                        position.Y = (double)next.Latitude;
                    }

                }
                if (road.over == true)
                {
                    finish = true;
                }
            }
        }
    }
}
