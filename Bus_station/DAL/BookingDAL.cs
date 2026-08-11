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
    public class BookingDAL : DBConnection
    {
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();
            string sql = "SELECT * FROM v_BookingDetails ORDER BY BookingDate DESC";
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
                throw new Exception("Error retrieving booking data: " + ex.Message);
            }
            return dt;
        }

        public DataTable GetRecentBooking()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT TOP 5 
                            BookingID, 
                            CustomerName, 
                            (Departure + ' to ' + Destination) AS Route, 
                            BookingDate, 
                            BookingStatus AS Status 
                        FROM v_BookingDetails 
                        ORDER BY BookingDate DESC";
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
                throw new Exception("Error retrieving recent bookings: " + ex.Message);
            }
            return dt;
        }

        public Booking GetById(int bookingID)
        {
            Booking bk = null;
            string sql = "SELECT * FROM Booking WHERE BookingID=@BookingID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BookingID", bookingID);
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bk = new Booking
                            {
                                BookingID = Convert.ToInt32(reader["BookingID"]),
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                ScheduleID = Convert.ToInt32(reader["ScheduleID"]),
                                BookingDate = Convert.ToDateTime(reader["BookingDate"]),
                                SeatNumber = Convert.ToInt32(reader["SeatNumber"]),
                                Status = reader["Status"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving booking by ID: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
            return bk;
        }

        public int Insert(Booking bk)
        {
            string sql = @"INSERT INTO Booking (CustomerID, ScheduleID, BookingDate, SeatNumber, Status)
                                 VALUES (@CustomerID, @ScheduleID, @BookingDate, @SeatNumber, @Status);
                                 SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", bk.CustomerID);
                    cmd.Parameters.AddWithValue("@ScheduleID", bk.ScheduleID);
                    cmd.Parameters.AddWithValue("@BookingDate", bk.BookingDate);
                    cmd.Parameters.AddWithValue("@SeatNumber", bk.SeatNumber);
                    cmd.Parameters.AddWithValue("@Status", bk.Status);
                    if (con.State == ConnectionState.Closed) con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting booking: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Update(Booking bk)
        {
            string sql = @"UPDATE Booking SET CustomerID=@CustomerID, ScheduleID=@ScheduleID, 
                           BookingDate=@BookingDate, SeatNumber=@SeatNumber, Status=@Status
                           WHERE BookingID=@BookingID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BookingID", bk.BookingID);
                    cmd.Parameters.AddWithValue("@CustomerID", bk.CustomerID);
                    cmd.Parameters.AddWithValue("@ScheduleID", bk.ScheduleID);
                    cmd.Parameters.AddWithValue("@BookingDate", bk.BookingDate);
                    cmd.Parameters.AddWithValue("@SeatNumber", bk.SeatNumber);
                    cmd.Parameters.AddWithValue("@Status", bk.Status);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating booking: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Delete(int bookingID)
        {
            string sql = "DELETE FROM Booking WHERE BookingID=@BookingID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BookingID", bookingID);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting booking: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT * FROM v_BookingDetails 
                                 WHERE CustomerName LIKE @Keyword 
                                    OR CustomerPhone LIKE @Keyword 
                                    OR Departure LIKE @Keyword 
                                    OR Destination LIKE @Keyword 
                                    OR BusNumber LIKE @Keyword
                                 ORDER BY BookingDate DESC";
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
                throw new Exception("Error searching booking data: " + ex.Message);
            }
            return dt;
        }

        public bool IsSeatBooked(int scheduleID, int seatNumber)
        {
            string sql = "SELECT COUNT(*) FROM Booking WHERE ScheduleID = @ScheduleID AND SeatNumber = @SeatNumber AND Status = 'Confirmed'";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@ScheduleID", scheduleID);
                    cmd.Parameters.AddWithValue("@SeatNumber", seatNumber);
                    if (con.State == ConnectionState.Closed) con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking seat booking: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public DataTable GetDashboardStats()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
                            (SELECT COUNT(*) FROM Customer) AS TotalCustomers,
                            (SELECT COUNT(*) FROM Bus) AS TotalBuses,
                            (SELECT COUNT(*) FROM Bus WHERE Status = 'Available') AS ActiveBuses,
                            (SELECT COUNT(*) FROM Driver) AS TotalDrivers,
                            (SELECT COUNT(*) FROM Booking WHERE CAST(BookingDate AS DATE) = CAST(GETDATE() AS DATE)) AS TodayBookings,
                            ISNULL((SELECT SUM(Amount) FROM Payment WHERE Status = 'Paid'), 0.00) AS TotalRevenue";
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
                throw new Exception("Error retrieving dashboard statistics: " + ex.Message);
            }
            return dt;
        }
    }
}
