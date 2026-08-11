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
    public class ScheduleDAL : DBConnection
    {
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();
            string sql = "SELECT * FROM v_ScheduleDetails ORDER BY DepartureTime DESC";
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
                throw new Exception("Error retrieving schedule data: " + ex.Message);
            }
            return dt;
        }

        public DataTable GetTodayBusTimetable()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
                            BusNumber AS BusID, 
                            (Departure + ' to ' + Destination) AS Route, 
                            DepartureTime, 
                            'Confirmed' AS CurrentStatus 
                        FROM v_ScheduleDetails 
                        WHERE CAST(DepartureTime AS DATE) = CAST(GETDATE() AS DATE)
                        ORDER BY DepartureTime ASC";
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
                throw new Exception("Error retrieving today's bus timetable: " + ex.Message);
            }
            return dt;
        }

        public Schedule GetById(int scheduleID)
        {
            Schedule schedule = null;
            string sql = "SELECT * FROM Schedule WHERE ScheduleID=@ScheduleID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@ScheduleID", scheduleID);
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            schedule = new Schedule
                            {
                                ScheduleID = Convert.ToInt32(reader["ScheduleID"]),
                                BusID = Convert.ToInt32(reader["BusID"]),
                                DriverID = Convert.ToInt32(reader["DriverID"]),
                                RouteID = Convert.ToInt32(reader["RouteID"]),
                                DepartureTime = Convert.ToDateTime(reader["DepartureTime"]),
                                ArrivalTime = Convert.ToDateTime(reader["ArrivalTime"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving schedule data by ID: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
            return schedule;
        }

        public bool Insert(Schedule schedule)
        {
            string sql = @"INSERT INTO Schedule (BusID, DriverID, RouteID, DepartureTime, ArrivalTime) 
                                 VALUES (@BusID, @DriverID, @RouteID, @DepartureTime, @ArrivalTime)";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BusID", schedule.BusID);
                    cmd.Parameters.AddWithValue("@DriverID", schedule.DriverID);
                    cmd.Parameters.AddWithValue("@RouteID", schedule.RouteID);
                    cmd.Parameters.AddWithValue("@DepartureTime", schedule.DepartureTime);
                    cmd.Parameters.AddWithValue("@ArrivalTime", schedule.ArrivalTime);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting schedule: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Update(Schedule schedule)
        {
            string sql = @"UPDATE Schedule 
                           SET BusID = @BusID, DriverID = @DriverID, RouteID = @RouteID, 
                               DepartureTime = @DepartureTime, ArrivalTime = @ArrivalTime 
                           WHERE ScheduleID = @ScheduleID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BusID", schedule.BusID);
                    cmd.Parameters.AddWithValue("@DriverID", schedule.DriverID);
                    cmd.Parameters.AddWithValue("@RouteID", schedule.RouteID);
                    cmd.Parameters.AddWithValue("@DepartureTime", schedule.DepartureTime);
                    cmd.Parameters.AddWithValue("@ArrivalTime", schedule.ArrivalTime);
                    cmd.Parameters.AddWithValue("@ScheduleID", schedule.ScheduleID);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating schedule: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Delete(int scheduleID)
        {
            string sql = "DELETE FROM Schedule WHERE ScheduleID = @ScheduleID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@ScheduleID", scheduleID);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting schedule: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT * FROM v_ScheduleDetails 
                         WHERE CAST(ScheduleID AS NVARCHAR) LIKE @Keyword 
                            OR BusNumber LIKE @Keyword 
                            OR DriverName LIKE @Keyword 
                            OR Departure LIKE @Keyword 
                            OR Destination LIKE @Keyword 
                         ORDER BY ScheduleID DESC";
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
                throw new Exception("Error searching schedule data: " + ex.Message);
            }
            return dt;
        }

        public int GetAvailableSeat(int scheduleID)
        {
            string sql = @"SELECT ISNULL(b.TotalSeat, 0) - ISNULL((SELECT COUNT(*) FROM Booking WHERE ScheduleID = @ScheduleID AND Status = 'Confirmed'), 0)
                                 FROM Schedule s
                                 INNER JOIN Bus b ON s.BusID = b.BusID
                                 WHERE s.ScheduleID = @ScheduleID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@ScheduleID", scheduleID);
                    if (con.State == ConnectionState.Closed) con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving schedule data by bus: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }
    }
}
