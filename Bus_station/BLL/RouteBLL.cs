using Bus_station.DAL;
using Bus_station.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.BLL
{
    public class RouteBLL
    {
        private readonly RouteDAL _routeDAL = new DAL.RouteDAL();
        public DataTable GetAllRoute() => _routeDAL.GetAll();
        public Route GetRouteById(int routeId)
        {
            if (routeId <= 0)
                throw new ArgumentException("Route ID must be greater than zero.");
            return _routeDAL.GetById(routeId);
        }
        private void ValidateRoute(Route route)
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route), "Route data cannot be null.");
            if (string.IsNullOrWhiteSpace(route.Departure))
                throw new ArgumentException("Departure cannot be empty.");
            if (string.IsNullOrWhiteSpace(route.Destination))
                throw new ArgumentException("Destination cannot be empty.");
            if (route.Distance <= 0)
                throw new ArgumentException("Distance must be greater than zero.");
            if (route.Price < 0)
                throw new ArgumentException("Price cannot be negative.");
        }
        public bool Insert(Route route)
        {
            ValidateRoute(route);
            return _routeDAL.Insert(route);
        }
        public bool Update(Route route)
        {
            if (route.RouteID <= 0) throw new ArgumentException("Invalid Route ID.");
            ValidateRoute(route);
            return _routeDAL.Update(route);
        }
        public bool Delete(int routeId)
        {
            if (routeId <= 0) throw new ArgumentException("Invalid Route ID.");
            return _routeDAL.Delete(routeId);
        }
        public DataTable Search(string keyword) => _routeDAL.Search(keyword);
    }
}
