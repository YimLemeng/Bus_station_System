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
    public class DriverBLL
    {
        private DriverDAL _driverDAL = new DriverDAL();
        public DataTable GetAllDriver() => _driverDAL.GetAll();
        public bool Insert(Driver drv)
        {
            if (drv == null) throw new ArgumentNullException(nameof(drv), "Driver cannot be null");
            if (string.IsNullOrWhiteSpace(drv.DriverName)) throw new ArgumentException("Driver Name is required.");
            return _driverDAL.Insert(drv);
        }
        public bool Update(Driver drv)
        {
            if (drv.DriverID <= 0) throw new ArgumentException("Invalid Driver ID for update");
            return _driverDAL.Update(drv);
        }
        public bool Delete(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Driver ID for deletion");
            return _driverDAL.Delete(id);
        }

        public DataTable Search(string keyword) => _driverDAL.Search(keyword);
    }
}
