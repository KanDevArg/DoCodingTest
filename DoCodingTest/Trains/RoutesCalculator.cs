using System;
using System.Collections.Generic;
using System.Linq;

namespace CandidateTest.TrainsRoutes
{ 
    public class RoutesCalculator
    {
        private Dictionary<string, Station> stations;
        private readonly List<string> trips;
        public RoutesCalculator()
        {
            trips = new List<string>();
            SetStationsDirectGraph();
        }
        
        public string CalculateDistance(string path)
        {
            var totalDistance = DistanceCalculator(path);
            return totalDistance > 0 ? totalDistance.ToString() : "NO SUCH ROUTE";
        }
        
        /// <summary>
        /// Given a route, such as A-B-C, it will the return the sum of the distances between each station on the path.
        /// If route can not be found, 0 will be returned. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private int DistanceCalculator(string path)
        {
            var stationNames = path.Split("-");
            var distance = 0;

            if (!stations.ContainsKey(stationNames[0]))
            {
                return 0;
            }
            
            for (var i = 0; i < stationNames.Length -1; i++)
            {
                var s = stations[stationNames[i]];
                if (s.Routes.ContainsKey(stationNames[i+1]))
                {
                    distance += s.Routes[stationNames[i+1]];
                }else
                {
                    return 0;
                }
            }

            return distance;
        }
        
        public int GetNumberOfTripsBetweenStations(string StationStart, string StationEnd, int maxStops, bool matchMaxStops)
        {
            trips.Clear();
            NumberOfTripsCalculator(StationStart, StationEnd, maxStops, matchMaxStops, "");
            foreach (var path in trips)
            {
                Console.WriteLine($"Trip: {path}  TotalDistance:{DistanceCalculator(path)}");
            }
            Console.WriteLine("---------------------");
            return trips.Count;
        }
        
        public string GetShortestTripBetweenStations(string StationStart, string StationEnd, int maxStops, bool matchMaxStops)
        {
            var pathsAndDistance = new Dictionary<string, int>();
            trips.Clear();
            NumberOfTripsCalculator(StationStart, StationEnd, maxStops, matchMaxStops, "");
            foreach (var path in trips)
            {
                if (!pathsAndDistance.ContainsKey(path)) {
                    pathsAndDistance.Add(path, DistanceCalculator(path));    
                }
                Console.WriteLine($"Trip: {path}  TotalDistance:{DistanceCalculator(path)}");
            }
            Console.WriteLine("---------------------");

            var myList = pathsAndDistance.ToList();
            myList.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));
            
            
            return trips.Count > 0 ? myList.First().Key : string.Empty;
        }
        
        public int GetAllPathsLimitByTotalDistance(string StationStart, string StationEnd, int totalDistanceLimit)
        {
            trips.Clear();
            NumberOfTripsCalculator(StationStart, StationEnd, "", totalDistanceLimit);
            foreach (var path in trips)
            {
                Console.WriteLine($"Trip: {path}  TotalDistance:{DistanceCalculator(path)}");
            }
            Console.WriteLine("---------------------");
            return trips.Count;
        }

        /// <summary>
        /// Calculate the number of feasible trips between 2 stations. Anchor for recursion is determined by [totalDistanceLimit]
        /// param value.
        /// </summary>
        /// <param name="StationStart"></param>
        /// <param name="StationEnd"></param>
        /// <param name="piggybackPath"></param>
        /// <param name="totalDistanceLimit"></param>
        private void NumberOfTripsCalculator(string StationStart, string StationEnd, string piggybackPath, int totalDistanceLimit)
        {
            var trimmedPiggyBackPath = piggybackPath.TrimEnd('-');
            var arr = trimmedPiggyBackPath.Split("-");
            var distance = DistanceCalculator(trimmedPiggyBackPath);
            
            if (distance > totalDistanceLimit) return;
                    
            if (arr.Length >1  && arr[^1] == StationEnd &&  distance < totalDistanceLimit) {
                if (!trips.Contains(trimmedPiggyBackPath)) {
                    trips.Add(trimmedPiggyBackPath);    
                }
            }
            
            var start = stations[StationStart];
            if (start.Routes.Count <= 0) return;
            
            foreach (var route in start.Routes) {
                NumberOfTripsCalculator(route.Key, StationEnd, piggybackPath.Length == 0 ?  start.Name + "-" : piggybackPath + start.Name + "-", totalDistanceLimit);
            }
        }
        
        /// <summary>
        /// Calculate the number of feasible trips between 2 stations. Anchor for recursion is determined by [maxStops] and
        /// [matchMaxStops] params values.
        /// </summary>
        /// <param name="StationStart"></param>
        /// <param name="StationEnd"></param>
        /// <param name="maxStops"></param>
        /// <param name="matchMaxStops"></param>
        /// <param name="piggybackPath"></param>
        private void NumberOfTripsCalculator(string StationStart, string StationEnd, int maxStops, bool matchMaxStops, string piggybackPath)
        {
            var trimmedPiggyBackPath = piggybackPath.TrimEnd('-');
            var arr = trimmedPiggyBackPath.Split("-");

            if (arr.Length > 2) {
                if (arr.Length > maxStops + 2) return;
                
                if (arr[^1] == StationEnd && (matchMaxStops && arr.Length == maxStops + 1 || !matchMaxStops && arr.Length <= maxStops + 1)) {
                    if (!trips.Contains(trimmedPiggyBackPath)) {
                        trips.Add(trimmedPiggyBackPath);
                    }
                    return;
                }
            }
            var start = stations[StationStart];

            if (start.Routes.Count <= 0) return;
            
            foreach (var route in start.Routes)
            {
                NumberOfTripsCalculator(route.Key, 
                    StationEnd, 
                    maxStops, 
                    matchMaxStops, 
                    piggybackPath.Length == 0 ?  start.Name + "-" : piggybackPath + start.Name + "-");
            }
            // Parallel.ForEach(start.Routes, route =>
            // {
            //     NumberOfTripsCalculator(route.Key, StationEnd, maxStops, matchMaxStops, piggyback.Length == 0 ?  start.Name + "-" : piggyback + start.Name + "-");
            // });
        }
        
        private void SetStationsDirectGraph()
        {
            stations = new Dictionary<string, Station>
            {
                {"A", new Station() {Name = "A", Routes = new Dictionary<string, int>() {{"B", 5}, {"D", 5}, {"E", 7},}}},
                {"B", new Station() {Name = "B", Routes = new Dictionary<string, int>() {{"C", 4},}}},
                {"C", new Station() {Name = "C", Routes = new Dictionary<string, int>() {{"D", 8}, {"E", 2},}}},
                {"D", new Station() {Name = "D", Routes = new Dictionary<string, int>() {{"C", 8}, {"E", 6},}}},
                {"E", new Station() {Name = "E", Routes = new Dictionary<string, int>() {{"B", 3},}}}
            };
        }
    }
}