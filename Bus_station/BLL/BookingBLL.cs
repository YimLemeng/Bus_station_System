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
    public class BookingBLL
    {
        private readonly BookingDAL _bookingDAL = new BookingDAL();
        private readonly ScheduleDAL _scheduleDAL = new ScheduleDAL();
        private readonly BusDAL _busDAL = new BusDAL();

        public DataTable GetAllBooking() => _bookingDAL.GetAll();
        public DataTable GetRecentBookings() => _bookingDAL.GetRecentBooking();
        public DataTable GetDashboardStats() => _bookingDAL.GetDashboardStats();

        public Booking GetBookingById(int bookingID)
        {
            if (bookingID <= 0)
                throw new ArgumentException("Booking ID must be greater than zero.");
            return _bookingDAL.GetById(bookingID);
        }
        public int InsertBooking(Booking bk)
        {
            ValidateBooking(bk, isNew: true);
            return _bookingDAL.Insert(bk);
        }
        public bool UpdateBooking(Booking bk)
        {
            if (bk.BookingID <= 0) throw new ArgumentException("Booking ID must be greater than zero for update.");
            ValidateBooking(bk, isNew: false);
            return _bookingDAL.Update(bk);
        }
        public bool DeleteBooking(int bookingID)
        {
            if (bookingID <= 0) throw new ArgumentException("Booking ID must be greater than zero for deletion.");
            return _bookingDAL.Delete(bookingID);
        }
        public DataTable SearchBooking(string searchTerm) => _bookingDAL.Search(searchTerm);

        public void ValidateBooking(Booking bk, bool isNew)
        {
            if (bk == null)
                throw new ArgumentNullException(nameof(bk), "Booking data cannot be null.");
            if (bk.CustomerID <= 0)
                throw new ArgumentException("Please select a valid Customer.");
            if (bk.ScheduleID <= 0)
                throw new ArgumentException("Please select a valid Schedule.");
            if (bk.SeatNumber <= 0)
                throw new ArgumentException("Seat number must be greater than zero.");
            if (string.IsNullOrWhiteSpace(bk.Status) || (bk.Status != "Confirmed" && bk.Status != "Cancelled" && bk.Status != "Pending"))
                throw new ArgumentException("Booking status must be 'Confirmed', 'Cancelled', or 'Pending'.");

            // Retrieve Schedule to check Bus capacity
            Schedule sched = _scheduleDAL.GetById(bk.ScheduleID);
            if (sched == null)
                throw new Exception("Selected schedule does not exist.");
            Bus bus = _busDAL.GetById(sched.BusID);
            if (bus == null)
                throw new Exception("Bus assigned to this schedule does not exist.");
            if (bk.SeatNumber > bus.TotalSeat)
                throw new ArgumentException($"Seat number {bk.SeatNumber} exceeds the assigned bus capacity of {bus.TotalSeat} seats.");

            // Check if the seat is already booked (for new bookings or seat changes in existing bookings)
            if (bk.Status == "Confirmed")
            {
                bool isConflict = false;
                if (isNew)
                {
                    isConflict = _bookingDAL.IsSeatBooked(bk.ScheduleID, bk.SeatNumber);
                }
                else
                {
                    Booking current = _bookingDAL.GetById(bk.BookingID);
                    if (current != null && (current.ScheduleID != bk.ScheduleID || current.SeatNumber != bk.SeatNumber || current.Status != "Confirmed"))
                    {
                        isConflict = _bookingDAL.IsSeatBooked(bk.ScheduleID, bk.SeatNumber);
                    }
                }
                if (isConflict)
                {
                    throw new ArgumentException($"Seat number {bk.SeatNumber} is already booked and confirmed for this schedule.");
                }
            }
        }
    }
}
