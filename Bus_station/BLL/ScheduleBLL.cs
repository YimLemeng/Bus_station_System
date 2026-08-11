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
    public class ScheduleBLL
    {
        private readonly ScheduleDAL _scheduleDAL = new ScheduleDAL();
        public DataTable GetAllSchedule() => _scheduleDAL.GetAll();
        public DataTable GetTodayBusTimetable() => _scheduleDAL.GetTodayBusTimetable();
        public Schedule GetScheduleById(int scheduleId)
        {
            if (scheduleId <= 0)
                throw new ArgumentException("Schedule ID must be greater than zero.");
            return _scheduleDAL.GetById(scheduleId);
        }
        public void ValidateSchedule(Schedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule), "Schedule data cannot be null.");
            if (schedule.BusID <= 0)
                throw new ArgumentException("Invalid Bus ID.");
            if (schedule.DriverID <= 0)
                throw new ArgumentException("Invalid Driver ID.");
            if (schedule.RouteID <= 0)
                throw new ArgumentException("Invalid Route ID.");
            if (schedule.DepartureTime >= schedule.ArrivalTime)
                throw new ArgumentException("Departure time must be before arrival time.");
        }
        public bool Insert(Schedule schedule)
        {
            ValidateSchedule(schedule);
            return _scheduleDAL.Insert(schedule);
        }
        public bool Update(Schedule schedule) { 
            if (schedule.ScheduleID <= 0) throw new ArgumentException("Invalid Schedule ID.");
            ValidateSchedule(schedule);
            return _scheduleDAL.Update(schedule);
        }
        public bool Delete(int scheduleId)
        {
            if (scheduleId <= 0) throw new ArgumentException("Invalid Schedule ID.");
            return _scheduleDAL.Delete(scheduleId);
        }
        public DataTable Search(string keyword) => _scheduleDAL.Search(keyword);
        public int GetAvailableSeats(int shcduleID)
        {
            if (shcduleID <= 0) throw new ArgumentException("Invalid Route ID.");
            return _scheduleDAL.GetAvailableSeat(shcduleID);
        }
    }
}
