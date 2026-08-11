using Bus_station.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace Bus_station.DAL
{
    public class DriverDAL : DBConnection
    {
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();
            string query = "SELECT DriverID, DriverName, Phone, LicenseNumber, Experience FROM Driver";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving drivers: " + ex.Message);
            }
            return dt;
        }

        public bool Insert (Driver drv)
        {
            string query = "INSERT INTO Driver (DriverName, Phone, LicenseNumber, Experience) VALUES (@DriverName, @Phone, @LicenseNumber, @Experience)";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DriverName", drv.DriverName);
                    cmd.Parameters.AddWithValue("@Phone", drv.Phone);
                    cmd.Parameters.AddWithValue("@LicenseNumber", drv.LicenseNumber);
                    cmd.Parameters.AddWithValue("@Experience", drv.Experience);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting driver: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public bool Update (Driver drv)
        {
            string query = "UPDATE Driver SET DriverName = @DriverName, Phone = @Phone, LicenseNumber = @LicenseNumber, Experience = @Experience WHERE DriverID = @DriverID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DriverID", drv.DriverID);
                    cmd.Parameters.AddWithValue("@DriverName", drv.DriverName);
                    cmd.Parameters.AddWithValue("@Phone", drv.Phone);
                    cmd.Parameters.AddWithValue("@LicenseNumber", drv.LicenseNumber);
                    cmd.Parameters.AddWithValue("@Experience", drv.Experience);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating driver: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public bool Delete (int id)
        {
            string query = "DELETE FROM Driver WHERE DriverID = @DriverID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DriverID", id);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting driver: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT * FROM Driver WHERE DriverName LIKE @Keyword OR Phone LIKE @Keyword OR LicenseNumber LIKE @Keyword";
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
                throw new Exception("Error searching drivers: " + ex.Message);
            }
            return dt;
        }
    }
}

