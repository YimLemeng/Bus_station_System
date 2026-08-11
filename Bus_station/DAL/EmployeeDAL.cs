using Bus_station.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.DAL
{
    public class EmployeeDAL : DBConnection
    {
        public DataSet GetAllEmployee()
        {
            DataSet ds = new DataSet();
            string query = "SELECT EmployeeID, FullName, Gender, DOB, Phone, Email, Address, Position, Salary FROM Employee";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving employees: " + ex.Message);
            }
            return ds;
        }

        public bool Insert(Employee emp)
        {
            string sql = @"INSERT INTO Employee (FullName, Gender, DOB, Phone, Email, Address, Position, Salary) 
                                 VALUES (@FullName, @Gender, @DOB, @Phone, @Email, @Address, @Position, @Salary)";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@FullName", emp.FullName);
                    cmd.Parameters.AddWithValue("@Gender", emp.Gender);
                    cmd.Parameters.AddWithValue("@DOB", emp.DOB);
                    cmd.Parameters.AddWithValue("@Phone", emp.Phone);
                    cmd.Parameters.AddWithValue("@Email", emp.Email);
                    cmd.Parameters.AddWithValue("@Address", emp.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Position", emp.Position);
                    cmd.Parameters.AddWithValue("@Salary", emp.Salary);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error insert employee: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Update(Employee emp)
        {
            string sql = @"UPDATE Employee SET FullName = @FullName, Gender = @Gender, DOB = @DOB, Phone = @Phone, Email = @Email, 
                                     Address = @Address, Position = @Position, Salary = @Salary 
                                        WHERE EmployeeID = @EmployeeID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", emp.EmployeeID);
                    cmd.Parameters.AddWithValue("@FullName", emp.FullName);
                    cmd.Parameters.AddWithValue("@Gender", emp.Gender);
                    cmd.Parameters.AddWithValue("@DOB", emp.DOB);
                    cmd.Parameters.AddWithValue("@Phone", emp.Phone);
                    cmd.Parameters.AddWithValue("@Email", emp.Email);
                    cmd.Parameters.AddWithValue("@Address", emp.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Position", emp.Position);
                    cmd.Parameters.AddWithValue("@Salary", emp.Salary);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error update employee" + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Delete(int id)
        {
            string sql = "DELETE FROM Employee WHERE EmployeeID = @EmployeeID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", id);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error delete employee: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT * FROM Employee 
                         WHERE FullName LIKE @Keyword OR Phone LIKE 
                            @Keyword OR Position LIKE @Keyword";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching employee: " + ex.Message);
            }
            return dt;
        }
    }
}
