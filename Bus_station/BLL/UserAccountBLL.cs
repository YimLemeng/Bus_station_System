using Bus_station.DAL;
using Bus_station.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.BLL
{
    public class UserAccountBLL
    {
        private readonly UserAccountDAL _userAccountDAL = new UserAccountDAL();
        public UserAccount Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Username and password cannot be empty.");
            return _userAccountDAL.Login(username.Trim(), password);
        }
    }
}
