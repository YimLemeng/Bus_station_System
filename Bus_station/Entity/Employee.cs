using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.Entity
{
    public class Employee
    {
        private int _employeeID;
        private string _fullName;
        private string _gender;
        private DateTime _dob;
        private string _phone;
        private string _email;
        private string _address;
        private string _position;
        private decimal _salary;

        public int EmployeeID { get => _employeeID; set => _employeeID = value; }
        public string FullName { get => _fullName; set => _fullName = value; }
        public string Gender { get => _gender; set => _gender = value; }
        public DateTime DOB { get => _dob; set => _dob = value; }
        public string Phone { get => _phone; set => _phone = value; }
        public string Email { get => _email; set => _email = value; }
        public string Address { get => _address; set => _address = value; }
        public string Position { get => _position; set => _position = value; }
        public decimal Salary { get => _salary; set => _salary = value; }
    }
}
