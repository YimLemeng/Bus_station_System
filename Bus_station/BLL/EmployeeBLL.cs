using Bus_station.DAL;
using Bus_station.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bus_station.BLL
{
    public class EmployeeBLL
    {
        private readonly EmployeeDAL _employeeDAL = new EmployeeDAL();
        public DataTable GetAllEmployees() => _employeeDAL.GetAllEmployee().Tables[0];

        public bool Insert(Employee emp)
        {
            ValidateEmployee(emp);
            return _employeeDAL.Insert(emp);
        }
        public bool Update(Employee emp)
        {
            if (emp.EmployeeID <= 0) throw new ArgumentException("Invalid Employee ID for update");
            ValidateEmployee(emp);
            return _employeeDAL.Update(emp);
        }
        public bool Delete(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Employee ID for deletion");
            return _employeeDAL.Delete(id);
        }
        public DataTable Searching(string keyword) => _employeeDAL.Search(keyword);

        private void ValidateEmployee(Employee emp)
        {
            if (emp == null)
                throw new ArgumentNullException(nameof(emp), "Employee data cannot be null.");
            if (string.IsNullOrWhiteSpace(emp.FullName))
                throw new ArgumentException("Full Name is required.");
            if (string.IsNullOrWhiteSpace(emp.Gender) || (emp.Gender != "Male" && emp.Gender != "Female" && emp.Gender != "Other"))
                throw new ArgumentException("Gender must be 'Male', 'Female', or 'Other'.");
            if (emp.DOB >= DateTime.Today)
                throw new ArgumentException("Date of Birth must be in the past.");
            if (string.IsNullOrWhiteSpace(emp.Phone) || !Regex.IsMatch(emp.Phone, @"^[0-9\-\+\s]{9,15}$"))
                throw new ArgumentException("Phone number must be a valid format between 9 to 15 digits.");
            if (string.IsNullOrWhiteSpace(emp.Email) || !Regex.IsMatch(emp.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Email address must be in a valid format.");
            if (string.IsNullOrWhiteSpace(emp.Position))
                throw new ArgumentException("Position is required.");
            if (emp.Salary < 0)
                throw new ArgumentException("Salary cannot be negative.");
        }
    }
}
