using Mapsui.Geometries;
using OsmSharp;
using OsmSharp.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmTraffic
{
    class LoadMapNodes
    {
		private Point minMapPoint;
		private Point maxMapPoint;

		public static Dictionary<long, Node> allNodes;
        public static FileStream fileStream;

        public LoadMapNodes()
        {
            allNodes = new Dictionary<long, Node>();
            fileStream = File.OpenRead(Variables.FILE_NAME);
            LoadAllNodes();
        }

        void LoadAllNodes()
        {
            using (var fileStream = File.OpenRead(Variables.FILE_NAME))
            {

                var src = new PBFOsmStreamSource(fileStream);
                var nodes = from osmGeo in src select osmGeo;

                foreach (var node in nodes)
                {
                    if (node.Type == OsmGeoType.Node)
                    {
                        allNodes.Add((long)node.Id, (Node)node);
                    }

                }
            }
        }

		public List<Node> LoadCity(string cityName)
		{
			var nodes = new List<Node>();
			var src = new PBFOsmStreamSource(fileStream);
			var cityBoundary = from osmGeo in src
							   where osmGeo.Type == OsmGeoType.Relation && osmGeo.Tags != null && osmGeo.Tags.Contains("boundary", "administrative") && osmGeo.Tags.Contains("name", cityName) && osmGeo.Tags.Contains("name:prefix", "miasto")
							   select osmGeo;
			var waysIds = new List<long>();
			foreach (var element in cityBoundary)
			{
				if (element.Type == OsmGeoType.Relation)
				{
					var relationTmp = (Relation)element;
					foreach (var member in relationTmp.Members)
					{
						if (member.Type == OsmGeoType.Way)
						{
							waysIds.Add(member.Id);
						}
					}
				}
			}
			var nodesIds = new List<long>();
			var waysCityBoundary = from osmGeo in src
								   where osmGeo.Type == OsmGeoType.Way && waysIds.Contains((long)osmGeo.Id)
								   select osmGeo;
			foreach (var way in waysCityBoundary)
			{
				if (way.Type == OsmGeoType.Way)
				{
					var wayTmp = (Way)way;
					foreach (var node in wayTmp.Nodes)
					{
						nodesIds.Add(node);
					}
				}
			}
			var nodesCityBoundary = from osmGeo in src
									where osmGeo.Type == OsmGeoType.Node && nodesIds.Contains((long)osmGeo.Id)
									select osmGeo;
			foreach (var node in nodesCityBoundary)
				if (node.Type == OsmGeoType.Node)
				{
					Node nodeTmp = (Node)node;
					if (!allNodes.ContainsKey((long)node.Id)) 
						allNodes.Add((long)node.Id, (Node)node);
					nodes.Add(nodeTmp);
				}
			return nodes;
		}
		public void LoadBoundaryPoints(Point maxPoint, Point minPoint)
		{
			maxMapPoint = maxPoint;
			minMapPoint = minPoint;
		}

		public List<Node> LoadTrafficLights()
		{
			var returningColection = new List<Node>();
			using (var fileStream = File.OpenRead(Variables.FILE_NAME))
			{
				var src = new PBFOsmStreamSource(fileStream).FilterBox((float)minMapPoint.X, (float)maxMapPoint.Y, (float)maxMapPoint.X, (float)minMapPoint.Y);
				var nodes = from osmGeo in src 
							where osmGeo.Tags != null && (osmGeo.Tags.Contains("crossing", "traffic_signals") || osmGeo.Tags.Contains("highway", "traffic_signals")) 
							select osmGeo;
				foreach (var node in nodes)
				{
					if (node.Type == OsmGeoType.Node)
					{
						returningColection.Add((Node)node);
						if (!allNodes.ContainsKey((long)node.Id)) 
							allNodes.Add((long)node.Id, (Node)node);

						//SimulationManager.trafficSignalsIds.Add((long)node.Id);
					}
				}
			}
			return returningColection;
		}

		public static List<Node> LoadNodesFromRouteId(List<long> ids)
		{
			var lost = 0;
			var returningCollection = new List<Node>();
			foreach (var id in ids)
			{
				if (!allNodes.ContainsKey(id))
				{
					lost++;
				}
				else returningCollection.Add(allNodes[id]);
			}
			return returningCollection;

		}

		public List<Node> LoadCityNodes()
		{
			var returningColection = new List<Node>();
			using (var fileStream = File.OpenRead(Variables.FILE_NAME))
			{
				var src = new PBFOsmStreamSource(fileStream).FilterBox((float)minMapPoint.X, (float)maxMapPoint.Y, (float)maxMapPoint.X, (float)minMapPoint.Y);
				var nodes = from osmGeo in src
							where osmGeo.Tags != null
							select osmGeo;
				foreach (var node in nodes)
				{
					if (node.Type == OsmGeoType.Node)
					{
						returningColection.Add((Node)node);
						if (!allNodes.ContainsKey((long)node.Id))
							allNodes.Add((long)node.Id, (Node)node);

						
					}
				}
			}
			return returningColection;
		}


	}
}
