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
    public class BusBLL
    {
        private readonly BusDAL _busDAL = new BusDAL();

        public DataTable GetAllBuses() => _busDAL.GetAll();

        public Bus GetBusById(int busID)
        {
            if (busID <= 0)
                throw new ArgumentException("Bus ID must be greater than zero.");
            return _busDAL.GetById(busID);
        } 
        
        public void ValidateBus(Bus bus)
        {
            if (bus == null)
                throw new ArgumentNullException(nameof(bus), "Bus data cannot be null.");
            if (string.IsNullOrWhiteSpace(bus.BusNumber))
                throw new ArgumentException("Bus number cannot be empty.");
            if (bus.TotalSeat <= 0)
                throw new ArgumentException("Total seat must be greater than zero.");
            if (string.IsNullOrWhiteSpace(bus.BusType))
                throw new ArgumentException("Bus type cannot be empty.");
            if (string.IsNullOrWhiteSpace(bus.Status) || (bus.Status != "Available" && bus.Status != "Maintenance" && bus.Status != "Out of Service"))
                throw new ArgumentException("Status must be 'Available', 'Maintenance', or 'Out of Service'.");
        }

        public bool InsertBus(Bus bus)
        {
            ValidateBus(bus);
            return _busDAL.Insert(bus);
        }

        public bool UpdateBus(Bus bus)
        {
            if (bus.BusID <= 0) throw new ArgumentException("Bus ID must be greater than zero for update.");
            ValidateBus(bus);
            return _busDAL.Update(bus);
        }

        public bool DeleteBus(int busID)
        {
            if (busID <= 0) throw new ArgumentException("Bus ID must be greater than zero for deletion.");
            return _busDAL.Delete(busID);
        }

        public DataTable SearchBus(string keywod) => _busDAL.Search(keywod);
    }
}
