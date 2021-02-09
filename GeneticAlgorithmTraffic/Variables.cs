using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmTraffic
{
    class Variables
    {
        public const string OSM_FILE_NAME = "swietokrzyskie-latest.osm.pbf";
        public const int MIN_IN_MILIS = 60000;
		public static Random random = new Random();
		public const string OSRM_URL = "http://router.project-osrm.org/route/v1/driving/";


		public static double DistanceInKmBetweenEarthCoordinates(double latitude1, double longitude1, double latitude2, double longitude2)
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
