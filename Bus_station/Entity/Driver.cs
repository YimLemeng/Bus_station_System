using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.Entity
{
    public class Driver
    {
        private int _driverID;
        private string _driverName;
        private string _phone;
        private string _licenseNumber;
        private int _experience;

        public int DriverID { get => _driverID; set => _driverID = value; }
        public string DriverName { get => _driverName; set => _driverName = value; }
        public string Phone { get => _phone; set => _phone = value; }
        public string LicenseNumber { get => _licenseNumber; set => _licenseNumber = value; }
        public int Experience { get => _experience; set => _experience = value; }

    }
}
