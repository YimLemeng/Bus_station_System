using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.Entity
{
    public class UserAccount
    {
        private int _userID;
        private int _employeeID;
        private string _username;
        private string _password;
        private string _role;

        public int UserID { get => _userID; set => _userID = value; }
        public int EmployeeID { get => _employeeID; set => _employeeID = value; }
        public string Username { get => _username; set => _username = value; }
        public string Password { get => _password; set => _password = value; }
        public string Role { get => _role; set => _role = value; }
    }
}
