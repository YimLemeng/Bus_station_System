using Bus_station.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.DAL
{
    public class UserAccountDAL : DBConnection
    {
        public UserAccount Login(string username, string password)
        {
            UserAccount user = null;
            string query = "SELECT * FROM UserAccount WHERE Username = @Username AND Password = @Password";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    if (con.State == System.Data.ConnectionState.Closed) con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new UserAccount
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                                Role = reader["Role"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login: " + ex.Message);
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open) con.Close();
            }
            return user;
        }
    }
}
