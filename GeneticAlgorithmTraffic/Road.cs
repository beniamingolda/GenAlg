using Mapsui.Geometries;
using Newtonsoft.Json.Linq;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmTraffic
{
    class Road
    {
        public Point start;
        public Point end;
        public List<Node> roadNodes;
        public Stack<Node> ongoingRoad;
        public Node next; 
        public bool over = false;
        public Road()
        {
            roadNodes = new List<Node>();
            start = new Point();
            end = new Point();
            ongoingRoad = new Stack<Node>();
        }

        public Road(Point start, Point end)
        {
            this.start = start;
            this.end = end;
            roadNodes = GetRoute(start, end);
            ongoingRoad = new Stack<Node>();
            for(int i = roadNodes.Count - 1; i >= 0; i--)
            {
                ongoingRoad.Push(roadNodes[i]);
            }
        }

        public Node NextNode()
        {
            if (ongoingRoad.Count > 0)
            {
                if (ongoingRoad.Count == 1)
                {
                    over = true;
                }
                
                next = ongoingRoad.Pop();
            }
            return next;
            
        }


        public static List<Node> GetRoute(Point begining, Point ending)
        {
            string osrmResponse = "";
            string beginPointX = begining.X.ToString().Replace(",", ".");
            string beginPointY = begining.Y.ToString().Replace(",", ".");
            string endingPointX = ending.X.ToString().Replace(",", ".");
            string endingPointY = ending.Y.ToString().Replace(",", ".");
            string url = Variables.OSRM_URL + beginPointX + "," + beginPointY + ";" + endingPointX + "," + endingPointY + "?annotations=nodes";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                osrmResponse = reader.ReadToEnd();
            }
            JObject j = JObject.Parse(osrmResponse);
            List<long> ids = GetRouteNodes(j);
            List<Node> nodes = OsmDataManager.GetNodesByIdsFromRoute(ids);
            return nodes;
        }

        public static List<long> GetRouteNodes(JObject jObject)
        {
            List<long> ids = new List<long>();
            var routesJArray = jObject.GetValue("routes");
            JObject routesJObject = (JObject)routesJArray[0];
            var legsJArray = routesJObject.GetValue("legs");
            JObject legsJObject = (JObject)legsJArray[0];
            var nodesJArray = ((JObject)legsJObject.GetValue("annotation")).GetValue("nodes");
            foreach (var nodeID in nodesJArray)
            {
                ids.Add((long)nodeID);
            }
            return ids;
        }
    }
}
