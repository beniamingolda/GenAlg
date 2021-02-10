using OsmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmTraffic
{
    class Crossing
    {
        int id;
        public List<TrafficLight> trafficLights;
        public double changeTime;

        public Crossing(int id)
        {
            this.id = id;
            trafficLights = new List<TrafficLight>();
        }


        public List<Node> SetupTrafficLights(List<Node> trafficLightsList,double changeTime)
        {
            var trafficLightNode = trafficLightsList[0];
            this.changeTime = changeTime;//test
            trafficLights.Add(new TrafficLight(trafficLightNode,changeTime));
            trafficLightsList.Remove(trafficLightNode);
            double distance;

            foreach(var node in trafficLightsList)
            {
                distance = Variables.DistanceInKmBetweenEarthCoordinates((double)node.Latitude, (double)node.Longitude, (double)trafficLightNode.Latitude, (double)trafficLightNode.Longitude) * 1000;
                if (distance < 150)
                {
                    trafficLights.Add(new TrafficLight(node,changeTime));
                }
            }
            for(var i = 0; i < trafficLights.Count / 2; i++)
            {
                if (i * 2 < trafficLights.Count)
                {
                    trafficLights[i * 2].greenOn = false;
                    trafficLights[i * 2].greenStart = false;
                }
                if (i * 2 + 1 < trafficLights.Count)
                {
                    trafficLights[i * 2 + 1].greenOn = true;
                    trafficLights[i * 2 + 1].greenStart = true;
                }
            }
            //pętla?
            foreach(var tl in trafficLights)
            {
                trafficLightsList.Remove(tl.trafficLightNode);
            }
            return trafficLightsList;
        }

        public void ChangeLights()
        {
            foreach(var tl in trafficLights)
            {
                if (tl.greenOn == true)
                {
                    tl.greenOn = false;
                }
                else if (tl.greenOn == false)
                {
                    tl.greenOn = true;
                }
            }
        }

    }
}
