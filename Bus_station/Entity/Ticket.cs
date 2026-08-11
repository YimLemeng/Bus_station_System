using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.Entity
{
    public class Ticket
    {
        private int _ticketID;
        private int _bookingID;
        private DateTime _issueDate;
        private decimal _price;

        public int TicketID { get => _ticketID; set => _ticketID = value; }
        public int BookingID { get => _bookingID; set => _bookingID = value; }
        public DateTime IssueDate { get => _issueDate; set => _issueDate = value; }
        public decimal Price { get => _price; set => _price = value; }
    }
}
