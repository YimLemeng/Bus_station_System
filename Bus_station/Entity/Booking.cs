using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.Entity
{
    public class Booking
    {
        private int _bookingID;
        private int _customerID;
        private int _scheduleID;
        private DateTime _bookingDate;
        private int _seatNumber;
        private string _status;

        public int BookingID { get => _bookingID; set => _bookingID = value; }
        public int CustomerID { get => _customerID; set => _customerID = value; }
        public int ScheduleID { get => _scheduleID; set => _scheduleID = value; }
        public DateTime BookingDate { get => _bookingDate; set => _bookingDate = value; }
        public int SeatNumber { get => _seatNumber; set => _seatNumber = value; }
        public string Status { get => _status; set => _status = value; }
    }
}
