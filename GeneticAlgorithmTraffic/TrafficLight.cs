using OsmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmTraffic
{
    

    class TrafficLight
    {
        public Node trafficLightNode;
        //czas zmiany świateł
        public double lightChangeTime;
        //czy zielone
        public bool greenOn;
        //czy zaczyna od zielonego 
        public bool greenStart;

        public TrafficLight(Node node,double changeTime)
        {
            trafficLightNode = node;
            lightChangeTime = changeTime;
            greenOn = false;
            greenStart = false;
        }

    }
}
