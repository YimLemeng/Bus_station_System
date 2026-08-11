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
    public class CustomerBLL
    {
        private readonly CustomerDAL _customerDAL = new CustomerDAL();
        public DataTable GetAllCustomers() => _customerDAL.GetAll();
        public Customer GetCustomerById(int customerId)
        {
            if (customerId <= 0) throw new Exception("Invalid Customer ID.");
            return _customerDAL.GeById(customerId);
        }
        public bool Insert(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Name) || string.IsNullOrWhiteSpace(customer.Gender) || string.IsNullOrWhiteSpace(customer.Phone))
            {
                throw new Exception("Name, Gender, and Phone cannot be empty.");
            }
            return _customerDAL.Insert(customer);
        }
        public bool Update(Customer customer)
        {
            if (customer.CustomerID <= 0)
            {
                throw new Exception("Invalid Customer ID.");
            }
            return _customerDAL.Update(customer);
        }
        public bool Delete(int customerId)
        {
            if (customerId <= 0)
            {
                throw new Exception("Invalid Customer ID.");
            }
            return _customerDAL.Delete(customerId);
        }
        public DataTable Search(string keyword) => _customerDAL.Search(keyword);
        public int InsertAndGetId(Customer cust) => _customerDAL.InsertAndGetId(cust);
    }
}
