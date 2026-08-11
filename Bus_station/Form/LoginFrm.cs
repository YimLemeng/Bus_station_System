using Bus_station.BLL;
using Bus_station.Entity;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bus_station
{
    public partial class LoginFrm : Form
    {
        private readonly UserAccountBLL _userAccountBLL = new UserAccountBLL();
        public LoginFrm()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter your Username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your Password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            try
            {
                // Authenticate user through UserAccount Business Logic Layer
                UserAccount loggedInUser = _userAccountBLL.Login(username, password);
                if (loggedInUser != null)
                {
                    MessageBox.Show($"Welcome back, {loggedInUser.Username} ({loggedInUser.Role})!",
                                    "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Open the Dashboard and pass the authenticated UserAccount object
                    MainFrm main = new MainFrm(loggedInUser);
                    this.Hide();
                    main.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid Username or Password. Please try again.",
                                    "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during login: {ex.Message}",
                                "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    } 
}
