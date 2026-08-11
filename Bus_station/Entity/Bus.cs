using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.Entity
{
    public class Bus
    {
        private int _busID;
        private string _busNumber;
        private string _busType;
        private int _totalSeat;
        private string _status;

        public int BusID { get => _busID; set => _busID = value; }
        public string BusNumber { get => _busNumber; set => _busNumber = value; }
        public string BusType { get => _busType; set => _busType = value; }
        public int TotalSeat { get => _totalSeat; set => _totalSeat = value; }
        public string Status { get => _status; set => _status = value; }
    }
}
