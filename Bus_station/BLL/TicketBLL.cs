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
    public class TicketBLL
    {
        private readonly TicketDAL _ticketDAL = new TicketDAL();
        public DataTable GetAllTicket() => _ticketDAL.GetAll();
        public DataTable GetByBookingId(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Ticket ID.");
            return _ticketDAL.GetById(id);
        }

        private void ValidateTicket(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket), "Ticket data cannot be null.");
            if (ticket.BookingID <= 0)
                throw new ArgumentException("Please select a valid Booking.");
            if (ticket.Price < 0)
                throw new ArgumentException("Ticket price cannot be negative.");
        }
        public bool Insert(Ticket ticket)
        {
            ValidateTicket(ticket);
            return _ticketDAL.Insert(ticket);
        }
        public bool Update(Ticket ticket)
        {
            if (ticket.TicketID <= 0) throw new ArgumentException("Invalid Ticket ID.");
            ValidateTicket(ticket);
            return _ticketDAL.Update(ticket);
        }
        public bool Delete(int ticketId)
        {
            if (ticketId <= 0) throw new ArgumentException("Invalid Ticket ID.");
            return _ticketDAL.Delete(ticketId);
        }
        public DataTable Search(string keyword) => _ticketDAL.Search(keyword);
    }
}
