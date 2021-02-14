using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmTraffic
{
    class Variables
    {
        public const string FILE_NAME = "swietokrzyskie-latest.osm.pbf";
        public const int SIMULATION_TIME = 10000;
		public static Random random = new Random();
		public const string OSRM_URL = "http://localhost:5000/route/v1/driving/";
		//router.project-osrm.org
		//localhost:5000

		public static double DistanceInKm(double latitude1, double longitude1, double latitude2, double longitude2)
		{
			var earthRadiusKm = 6371;

			var degreesLatitude = DegreesToRadians(latitude2 - latitude1);
			var degreesLongitude = DegreesToRadians(longitude2 - longitude1);

			latitude1 = DegreesToRadians(latitude1);
			latitude2 = DegreesToRadians(latitude2);


			var a = Math.Sin(degreesLatitude / 2) * Math.Sin(degreesLatitude / 2) +
					Math.Sin(degreesLongitude / 2) * Math.Sin(degreesLongitude / 2) * Math.Cos(latitude1) * Math.Cos(latitude2);
			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			return earthRadiusKm * c;
		}
		public static double DegreesToRadians(double degrees)
		{
			return degrees * Math.PI / 180;
		}
	}
}
