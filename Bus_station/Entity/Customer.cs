using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.Entity
{
    public class Customer
    {
        private int _customerID;
        private string _name;
        private string _gender;
        private string _phone;
        private string _email;

        public int CustomerID { get => _customerID; set => _customerID = value; }
        public string Name { get => _name; set => _name = value; }
        public string Gender { get => _gender; set => _gender = value; }
        public string Phone { get => _phone; set => _phone = value; }
        public string Email { get => _email; set => _email = value; }
    }
}
