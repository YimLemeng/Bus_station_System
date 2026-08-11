using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.Entity
{
    public class Route
    {
        private int _routeID;
        private string _departure;
        private string _destination;
        private decimal _distance;
        private decimal _price;

        public int RouteID { get => _routeID; set => _routeID = value; }
        public string Departure { get => _departure; set => _departure = value; }
        public string Destination { get => _destination; set => _destination = value; }
        public decimal Distance { get => _distance; set => _distance = value; }
        public decimal Price { get => _price; set => _price = value; }
    }
}
