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
    public class RouteDAL : DBConnection
    {
        public DataTable GetAll() 
        {
            DataTable dt = new DataTable();
            string sql = "SELECT RouteID, Departure, Destination, Distance, Price FROM Route";
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
                throw new Exception("Error retrieving Route: " + ex.Message);
            }
            return dt;
        }

        public Route GetById(int routeId)
        {
            Route route = null;
            string sql = "SELECT RouteID, Departure, Destination, Distance, Price FROM Route WHERE RouteID = @RouteID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@RouteID", routeId);
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            route = new Route
                            {
                                RouteID = Convert.ToInt32(reader["RouteID"]),
                                Departure = reader["Departure"].ToString(),
                                Destination = reader["Destination"].ToString(),
                                Distance = Convert.ToDecimal(reader["Distance"]),
                                Price = Convert.ToDecimal(reader["Price"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving Route by ID: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
            return route;
        }

        public bool Insert(Route route)
        {
            string sql = "INSERT INTO Route (Departure, Destination, Distance, Price) VALUES (@Departure, @Destination, @Distance, @Price)";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Departure", route.Departure);
                    cmd.Parameters.AddWithValue("@Destination", route.Destination);
                    cmd.Parameters.AddWithValue("@Distance", route.Distance);
                    cmd.Parameters.AddWithValue("@Price", route.Price);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error insert Route: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Update(Route route)
        {
            string sql = "UPDATE Route SET Departure = @Departure, Destination = @Destination, Distance = @Distance, Price = @Price WHERE RouteID = @RouteID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@RouteID", route.RouteID);
                    cmd.Parameters.AddWithValue("@Departure", route.Departure);
                    cmd.Parameters.AddWithValue("@Destination", route.Destination);
                    cmd.Parameters.AddWithValue("@Distance", route.Distance);
                    cmd.Parameters.AddWithValue("@Price", route.Price);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error update Route: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public bool Delete(int routeId)
        {
            string sql = "DELETE FROM Route WHERE RouteID = @RouteID";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@RouteID", routeId);
                    if (con.State == ConnectionState.Closed) con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error delete Route: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT RouteID, Departure, Destination, Distance, Price FROM Route WHERE Departure LIKE @Keyword OR Destination LIKE @Keyword";
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
                throw new Exception("Error searching Route: " + ex.Message);
            }
            return dt;
        }
    }
}
