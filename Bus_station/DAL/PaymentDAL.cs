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
    public class PaymentDAL : DBConnection
    {
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();
            string sql = "SELECT * FROM vw_PaymentDetails ORDER BY PaymentID DESC";
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
                throw new Exception("Error retrieving payment details: " + ex.Message);
            }
            return dt;
        }

        public bool Insert(Payment pay)
        {
            string sql = "INSERT INTO Payment (BookingID, PaymentMethod, Amount, PaymentDate, Status) VALUES (@BookingID, @PaymentMethod, @Amount, @PaymentDate, @Status)";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BookingID", pay.BookingID);
                    cmd.Parameters.AddWithValue("@PaymentMethod", pay.PaymentMethod);
                    cmd.Parameters.AddWithValue("@Amount", pay.Amount);
                    cmd.Parameters.AddWithValue("@PaymentDate", pay.PaymentDate);
                    cmd.Parameters.AddWithValue("@Status", pay.Status);
                    if (con.State == ConnectionState.Closed) con.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0 && pay.Status == "Paid" && pay.BookingID > 0)
                    {
                        string syncSql = "UPDATE Booking SET Status = 'Confirmed' WHERE BookingID = @BookingID";
                        using (SqlCommand syncCmd = new SqlCommand(syncSql, con))
                        {
                            syncCmd.Parameters.AddWithValue("@BookingID", pay.BookingID);
                            syncCmd.ExecuteNonQuery();
                        }
                    }
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting payment: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Update(Payment pay)
        {
            string sql = @"UPDATE Payment 
                                 SET BookingID = @BookingID, 
                                     PaymentMethod = @PaymentMethod, 
                                     Amount = @Amount, 
                                     PaymentDate = @PaymentDate, 
                                     Status = @Status 
                                 WHERE PaymentID = @PaymentID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@PaymentID", pay.PaymentID);
                    cmd.Parameters.AddWithValue("@BookingID", pay.BookingID);
                    cmd.Parameters.AddWithValue("@PaymentMethod", pay.PaymentMethod);
                    cmd.Parameters.AddWithValue("@Amount", pay.Amount);
                    cmd.Parameters.AddWithValue("@PaymentDate", pay.PaymentDate);
                    cmd.Parameters.AddWithValue("@Status", pay.Status);
                    if (con.State == ConnectionState.Closed) con.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0 && pay.Status == "Paid" && pay.BookingID > 0)
                    {
                        string syncSql = "UPDATE Booking SET Status = 'Confirmed' WHERE BookingID = @BookingID";
                        using (SqlCommand syncCmd = new SqlCommand(syncSql, con))
                        {
                            syncCmd.Parameters.AddWithValue("@BookingID", pay.BookingID);
                            syncCmd.ExecuteNonQuery();
                        }
                    }
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating payment: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Delete(int paymentID)
        {
            string sql = "DELETE FROM Payment WHERE PaymentID=@PaymentID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@PaymentID", paymentID);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting payment: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT * FROM vw_PaymentDetails 
               WHERE CAST(PaymentID AS NVARCHAR) LIKE @keyword 
                  OR CAST(BookingID AS NVARCHAR) LIKE @keyword 
                  OR PaymentMethod LIKE @keyword 
                  OR CAST(Amount AS NVARCHAR) LIKE @keyword 
                  OR CAST(PaymentDate AS NVARCHAR) LIKE @keyword 
                  OR Status LIKE @keyword
               ORDER BY PaymentDate DESC";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching payment details: " + ex.Message);
            }
            return dt;
        }

        public DataTable GetRevenueReport()
        {
            DataTable dt = new DataTable();
            string sql = "SELECT * FROM v_RevenueReport";
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
                throw new Exception("Error retrieving revenue report: " + ex.Message);
            }
            return dt;
        }
    }
}
