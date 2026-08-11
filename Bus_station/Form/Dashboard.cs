using Bus_station.BLL;
using Bus_station.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bus_station
{
    public partial class MainFrm : Form
    {
        private Button currentButton;
        private Random random;
        private int tempIndex;
        private Form activeForm;
        private UserAccount _currentUser;

        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        public MainFrm(UserAccount user)
        {
            InitializeComponent();
            LoadDashboardStatistics();
            LoadDashboardTables();
            RefreshData();
            _currentUser = user;
            random = new Random();
            btnCloseChildForm.Visible = false;
            this.Text = string.Empty;
            this.ControlBox = false;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            if (_currentUser != null && _currentUser.Role != "Admin")
            {
                btnEmployee.Visible = false;
                btnReports.Visible = false;
                btnDriver.Visible = false;
                btnRoute.Visible = false;
            }
            this.Load += (s, e) => { MakeAllButtonsRounded(panelMenu, 15); RefreshData(); };
        }

        public void RefreshData()
        {
            LoadDashboardStatistics();
            LoadDashboardTables();  
        }

        private void LoadDashboardStatistics()
        {
            try
            {
                BookingBLL bookingBLL = new BookingBLL();
                DataTable dtstats = bookingBLL.GetDashboardStats();
                if (dtstats.Rows.Count > 0)
                {
                    DataRow row = dtstats.Rows[0];
                    lblTotalRevenue.Text = $"${Convert.ToDecimal(row["TotalRevenue"]):N2}";
                    lblTodayBookings.Text = row["TodayBookings"].ToString();
                    lblActiveBuses.Text = $"{row["ActiveBuses"]} / {row["TotalBuses"]}";
                    lblTotalCustomers.Text = row["TotalCustomers"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard statistics: " + ex.Message);
            }
        }

        private void LoadDashboardTables()
        {
            try
            {
                BookingBLL bookingBLL = new BookingBLL();
                ScheduleBLL scheduleBLL = new ScheduleBLL();
                dgvRecentBookings.DataSource = bookingBLL.GetRecentBookings();
                dgvBusTimetable.DataSource = scheduleBLL.GetTodayBusTimetable();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading tables: " + ex.Message);
            }
        }

        private void MakeButtonRounded(Button btn, int radius)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btn.Width, btn.Height, radius, radius));
        }

        private void MakeAllButtonsRounded(Control container, int radius)
        {
            foreach (Control c in container.Controls)
            {
                if (c is Button btn)
                {
                    MakeButtonRounded(btn, radius);
                }
                else if (c.HasChildren)
                {
                    MakeAllButtonsRounded(c, radius);
                }
            }
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private Color SelectThemColor()
        {
            int index = random.Next(ThemColor.ColorList.Count);
            while (tempIndex == index)
            {
                index = random.Next(ThemColor.ColorList.Count);
            }
            tempIndex = index;
            string color = ThemColor.ColorList[index];
            return ColorTranslator.FromHtml(color);
        }
        private void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {
                if (currentButton != (Button)btnSender)
                {
                    DisableButton();
                    Color color = SelectThemColor();
                    currentButton = (Button)btnSender;
                    currentButton.BackColor = color;
                    currentButton.ForeColor = Color.White;
                    currentButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    btnCloseChildForm.Visible = true;
                    MakeButtonRounded(currentButton, 25);
                }
            }
        }
        private void DisableButton()
        {
            foreach (Control previousBtn in panelMenu.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = Color.FromArgb(0, 118, 212);
                    previousBtn.ForeColor = Color.Gainsboro;
                    previousBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    MakeButtonRounded((Button)previousBtn, 25);
                }
            }
        }

        private void OpenChildForm(Form childForm, object btnSender)
        {
            if (activeForm != null) activeForm.Close();
            ActivateButton(btnSender);
            activeForm = childForm;
            childForm.FormClosed += (s, e) => {
                Reset();
                RefreshData();
            };
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            this.panelDesktop.Controls.Add(childForm);
            this.panelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            lblTitle.Text = childForm.Text;
        }

        private void Reset()
        {
            DisableButton();
            lblTitle.Text = "Dashboard";
            currentButton = null;
            btnCloseChildForm.Visible = false;
        }

        private void btnBus_Click(object sender, EventArgs e)
        {
            OpenChildForm(new BusFrm(_currentUser), sender);
        }

        private void btnCloseChildForm_Click(object sender, EventArgs e)
        {
            if (activeForm != null) activeForm.Close();
            Reset();
            RefreshData();
        }

        private void btnSchedules_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ScheduleFrm(_currentUser), sender);
        }

        private void btnBooking_Click(object sender, EventArgs e)
        {
            OpenChildForm(new BookingFrm(_currentUser), sender);
        }

        private void txtRoute_Click(object sender, EventArgs e)
        {
            OpenChildForm(new RouteFrm(_currentUser), sender);
        }

        private void btnTicket_Click(object sender, EventArgs e)
        {
            OpenChildForm(new TicketFrm(_currentUser), sender);
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            OpenChildForm(new PaymentFrm(), sender);
        }

        private void btnEmployee_Click(object sender, EventArgs e)
        {
            OpenChildForm(new EmployeeFrm(_currentUser), sender);
        }

        private void btnDriver_Click(object sender, EventArgs e)
        {
            OpenChildForm(new DriverFrm(), sender);
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            OpenChildForm(new CustomerFrm(), sender);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ReportFrm(), sender);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginFrm loginForm = new LoginFrm();
            this.Hide();
            loginForm.ShowDialog();
        }

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
             if (WindowState == FormWindowState.Normal) this.WindowState = FormWindowState.Maximized;
             else this.WindowState = FormWindowState.Normal;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
