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
    public class TicketDAL : DBConnection
    {
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT t.TicketID, t.BookingID, t.IssueDate, t.Price, 
                                        c.FullName AS CustomerName, r.Departure, r.Destination, sch.DepartureTime, b.SeatNumber
                                 FROM Ticket t 
                                 INNER JOIN Booking b ON t.BookingID = b.BookingID 
                                 INNER JOIN Customer c ON b.CustomerID = c.CustomerID 
                                 INNER JOIN Schedule sch ON b.ScheduleID = sch.ScheduleID 
                                 INNER JOIN Route r ON sch.RouteID = r.RouteID
                                 ORDER BY t.IssueDate DESC";
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
                throw new Exception("Error retrieving Ticket: " + ex.Message);
            }
            return dt;
        }

        public DataTable GetById(int id)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT t.TicketID, t.BookingID, t.IssueDate, t.Price, 
                                        c.FullName AS CustomerName, r.Departure, r.Destination, sch.DepartureTime, b.SeatNumber
                                 FROM Ticket t 
                                 INNER JOIN Booking b ON t.BookingID = b.BookingID 
                                 INNER JOIN Customer c ON b.CustomerID = c.CustomerID 
                                 INNER JOIN Schedule sch ON b.ScheduleID = sch.ScheduleID 
                                 INNER JOIN Route r ON sch.RouteID = r.RouteID
                                 WHERE t.TicketID = @TicketID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@TicketID", id);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving Ticket by ID: " + ex.Message);
            }
            return dt;
        }

        public bool Insert(Ticket ticket)
        {
            string sql = "INSERT INTO Ticket (BookingID, IssueDate, Price) VALUES (@BookingID, @IssueDate, @Price)";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BookingID", ticket.BookingID);
                    cmd.Parameters.AddWithValue("@IssueDate", ticket.IssueDate);
                    cmd.Parameters.AddWithValue("@Price", ticket.Price);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error insert Ticket: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Update(Ticket ticket)
        {
            string sql = "UPDATE Ticket SET BookingID = @BookingID, IssueDate = @IssueDate, Price = @Price WHERE TicketID = @TicketID";
            try
            {
                using(SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@TicketID", ticket.TicketID);
                    cmd.Parameters.AddWithValue("@BookingID", ticket.BookingID);
                    cmd.Parameters.AddWithValue("@IssueDate", ticket.IssueDate);
                    cmd.Parameters.AddWithValue("@Price", ticket.Price);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error update Ticket: " + ex.Message);
            }
            finally
            {
                if(con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Delete(int id)
        {
            string sql = "DELETE FROM Ticket WHERE TicketID = @TicketID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@TicketID", id);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error delete Ticket: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT * FROM vw_TicketDetails 
                   WHERE CAST(TicketID AS NVARCHAR) LIKE @Keyword 
                      OR CustomerName LIKE @Keyword 
                      OR Departure LIKE @Keyword 
                      OR Destination LIKE @Keyword 
                   ORDER BY IssueDate DESC";
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
                throw new Exception("Error searching Ticket: " + ex.Message);
            }
            return dt;
        }
    }
}
