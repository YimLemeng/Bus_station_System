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
    public class BusDAL : DBConnection
    {
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();
            string sql = "SELECT BusID, BusNumber, BusType, TotalSeat, Status FROM Bus";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving buses: " + ex.Message);
            }
            return dt;
        }

        public Bus GetById(int busID)
        {
            Bus bus = null;
            string sql = "SELECT BusID, BusNumber, BusType, TotalSeat, Status FROM Bus WHERE BusID = @BusID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BusID", busID);
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bus = new Bus
                            {
                                BusID = Convert.ToInt32(reader["BusID"]),
                                BusNumber = reader["BusNumber"].ToString(),
                                BusType = reader["BusType"].ToString(),
                                TotalSeat = Convert.ToInt32(reader["TotalSeat"]),
                                Status = reader["Status"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving bus by ID: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
            return bus;
        }

        public bool Insert(Bus bus)
        {
            string sql = "INSERT INTO Bus (BusNumber, BusType, TotalSeat, Status) VALUES (@BusNumber, @BusType, @TotalSeat, @Status)";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BusNumber", bus.BusNumber);
                    cmd.Parameters.AddWithValue("@BusType", bus.BusType);
                    cmd.Parameters.AddWithValue("@TotalSeat", bus.TotalSeat);
                    cmd.Parameters.AddWithValue("@Status", bus.Status);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting bus: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Update(Bus bus)
        {
            string sql = "UPDATE Bus SET BusNumber = @BusNumber, BusType = @BusType, TotalSeat = @TotalSeat, Status = @Status WHERE BusID = @BusID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BusID", bus.BusID);
                    cmd.Parameters.AddWithValue("@BusNumber", bus.BusNumber);
                    cmd.Parameters.AddWithValue("@BusType", bus.BusType);
                    cmd.Parameters.AddWithValue("@TotalSeat", bus.TotalSeat);
                    cmd.Parameters.AddWithValue("@Status", bus.Status);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating bus: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Delete(int busID)
        {
            string sql = "DELETE FROM Bus WHERE BusID = @BusID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BusID", busID);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting bus: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT * FROM Bus WHERE BusNumber LIKE @Keyword OR BusType LIKE @Keyword OR Status LIKE @Keyword";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching buses: " + ex.Message);
            }
            return dt;
        }
    }
}
