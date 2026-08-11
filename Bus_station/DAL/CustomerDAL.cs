using Bus_station.Entity;
using Microsoft.ReportingServices.Diagnostics.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.DAL
{
    public class CustomerDAL : DBConnection
    {
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();
            string sql = "SELECT CustomerID, FullName, Gender, Phone, Email FROM Customer";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving customers: " + ex.Message);
            }
            return dt;
        }

        public Customer GeById(int customerId)
        {
            Customer cust = null;
            string sql = "SELECT * FROM Customer WHERE CustomerID = @CustomerID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cust = new Customer
                            {
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                Name = reader["FullName"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Email = reader["Email"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving customer by ID: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
            return cust;
        }

        public bool Insert(Customer cust)
        {
            string sql = "INSERT INTO Customer (FullName, Gender, Phone, Email) VALUES (@FullName, @Gender, @Phone, @Email)";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@FullName", cust.Name);
                    cmd.Parameters.AddWithValue("@Gender", cust.Gender);
                    cmd.Parameters.AddWithValue("@Phone", cust.Phone);
                    cmd.Parameters.AddWithValue("@Email", cust.Email ?? (object)DBNull.Value);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting customer: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Update(Customer cust)
        {
            string sql = "UPDATE Customer SET FullName = @FullName, Gender = @Gender, Phone = @Phone, Email = @Email WHERE CustomerID = @CustomerID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", cust.CustomerID);
                    cmd.Parameters.AddWithValue("@FullName", cust.Name);
                    cmd.Parameters.AddWithValue("@Gender", cust.Gender);
                    cmd.Parameters.AddWithValue("@Phone", cust.Phone);
                    cmd.Parameters.AddWithValue("@Email", cust.Email ?? (object)DBNull.Value);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating customer: " + ex.Message);
            }
        }

        public bool Delete(int customerId)
        {
            string sql = "DELETE FROM Customer WHERE CustomerID = @CustomerID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting customer: " + ex.Message);
            }
        }

        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT * FROM Customer WHERE FullName LIKE @Keyword OR Phone LIKE @Keyword OR Email LIKE @Keyword";
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
                throw new Exception("Error searching customers: " + ex.Message);
            }
            return dt;
        }

        public int InsertAndGetId(Customer cust)
        {
            // 1. ឆែកមើលថាតើលេខទូរស័ព្ទនេះមានក្នុង Database រួចហើយឬនៅ?
            string checkSql = "SELECT CustomerID FROM Customer WHERE Phone = @Phone";
            using (SqlCommand checkCmd = new SqlCommand(checkSql, con))
            {
                checkCmd.Parameters.AddWithValue("@Phone", cust.Phone);
                if (con.State == ConnectionState.Closed) con.Open();
                object existingId = checkCmd.ExecuteScalar();

                if (existingId != null && existingId != DBNull.Value)
                {
                    return Convert.ToInt32(existingId); // បើមានរួចហើយ យក CustomerID ចាស់មកប្រើភ្លាម (មិនបាច់ Insert ស្ទួន)!
                }
            }

                string insertSql = @"INSERT INTO Customer (FullName, Gender, Phone, Email) 
                               VALUES (@FullName, @Gender, @Phone, @Email);
                               SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlCommand cmd = new SqlCommand(insertSql, con))
                {
                    cmd.Parameters.AddWithValue("@FullName", cust.Name);
                    cmd.Parameters.AddWithValue("@Gender", cust.Gender ?? "Other");
                    cmd.Parameters.AddWithValue("@Phone", cust.Phone);
                    cmd.Parameters.AddWithValue("@Email", cust.Email ?? "");
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting customer and getting ID: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }
    }
}
