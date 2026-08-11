using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.Entity
{
    public class Schedule
    {
        private int _scheduleID;
        private int _busID;
        private int _driverID;
        private int _routeID;
        private DateTime _departureTime;
        private DateTime _arrivalTime;

        public int ScheduleID { get => _scheduleID; set => _scheduleID = value; }
        public int BusID { get => _busID; set => _busID = value; }
        public int DriverID { get => _driverID; set => _driverID = value; }
        public int RouteID { get => _routeID; set => _routeID = value; }
        public DateTime DepartureTime { get => _departureTime; set => _departureTime = value; }
        public DateTime ArrivalTime { get => _arrivalTime; set => _arrivalTime = value; }
    }
}
