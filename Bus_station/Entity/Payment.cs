using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.Entity
{
    public class Payment
    {
        private int _paymentID;
        private int _bookingID;
        private string _paymentMethod;
        private decimal _amount;
        private DateTime _paymentDate;
        private string _status;

        public int PaymentID { get => _paymentID; set => _paymentID = value; }
        public int BookingID { get => _bookingID; set => _bookingID = value; }
        public string PaymentMethod { get => _paymentMethod; set => _paymentMethod = value; }
        public decimal Amount { get => _amount; set => _amount = value; }
        public DateTime PaymentDate { get => _paymentDate; set => _paymentDate = value; }
        public string Status { get => _status; set => _status = value; }
    }
}
