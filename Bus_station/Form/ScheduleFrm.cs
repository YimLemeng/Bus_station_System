using Bus_station.BLL;
using Bus_station.Entity;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
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
    public partial class ScheduleFrm : Form
    {
        private readonly UserAccount _currentUser;
        private readonly ScheduleBLL _scheduleBLL = new ScheduleBLL();
        private readonly BusBLL _busBLL = new BusBLL();
        private readonly DriverBLL _driverBLL = new DriverBLL();
        private readonly RouteBLL _routeBLL = new RouteBLL();

        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        public ScheduleFrm(UserAccount user)
        {
            InitializeComponent();
            _currentUser = user;
            txtScheduleID.Enabled = false;
            ApplyRolePrivileges();
            LoadDropdowns();
            LoadSchedules();
            ClearForm();
            cboDriver.DropDownStyle = ComboBoxStyle.DropDownList;
            cboBus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboRoute.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void ScheduleFrm_Load(object sender, EventArgs e)
        {
            txtSearch.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtSearch.Width, txtSearch.Height, 25, 25));
        }

        private void ApplyRolePrivileges()
        {
            btnDelete.Visible = (_currentUser != null && _currentUser.Role == "Admin");
        }

        private void LoadDropdowns()
        {
            try
            {
                DataTable dtBuses = _busBLL.GetAllBuses();
                cboBus.DataSource = dtBuses;
                cboBus.DisplayMember = "BusNumber";
                cboBus.ValueMember = "BusID";

                DataTable dtDrivers = _driverBLL.GetAllDriver();
                cboDriver.DataSource = dtDrivers;
                cboDriver.DisplayMember = "DriverName";
                cboDriver.ValueMember = "DriverID";

                DataTable dtRoutes = _routeBLL.GetAllRoute();
                dtRoutes.Columns.Add("RouteDetails", typeof(string), "Departure + ' to ' + Destination + ' ($' + Price + ')'");
                cboRoute.DataSource = dtRoutes;
                cboRoute.DisplayMember = "RouteDetails";
                cboRoute.ValueMember = "RouteID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load lookup data: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSchedules()
        {
            try
            {
                DataTable dt = _scheduleBLL.GetAllSchedule();
                dgvSchedule.DataSource = dt;
                string[] columnsToHide = {
                "BusID", "DriverID", "RouteID", "DriverPhone",
                "Distance", "Price", "BusType", "TotalSeat"
                };
                foreach (string col in columnsToHide)
                {
                    if (dgvSchedule.Columns.Contains(col))
                    {
                        dgvSchedule.Columns[col].Visible = false; 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load schedule records: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtScheduleID.Clear();
            if (cboBus.Items.Count > 0) cboBus.SelectedIndex = -1;
            if (cboDriver.Items.Count > 0) cboDriver.SelectedIndex = -1;
            if (cboRoute.Items.Count > 0) cboRoute.SelectedIndex = -1;
            dtpDepartureTime.Value = DateTime.Now.AddHours(1);
            dtpArrivalTime.Value = DateTime.Now.AddHours(6);
            txtSearch.Clear();
            btnSave.Enabled = true;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboBus.SelectedValue == null || cboDriver.SelectedValue == null || cboRoute.SelectedValue == null)
            {
                MessageBox.Show("Please select a Bus, Driver, and Route.");
                return;
            }
            try
            {
                Schedule s = new Schedule
                {
                    ScheduleID = 0,
                    BusID = Convert.ToInt32(cboBus.SelectedValue),
                    DriverID = Convert.ToInt32(cboDriver.SelectedValue),
                    RouteID = Convert.ToInt32(cboRoute.SelectedValue),
                    DepartureTime = dtpDepartureTime.Value,
                    ArrivalTime = dtpArrivalTime.Value
                };
                if (_scheduleBLL.Insert(s))
                {
                    MessageBox.Show("Schedule created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSchedules();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to save schedule.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtScheduleID.Text))
            {
                MessageBox.Show("Please select a schedule from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cboBus.SelectedValue == null || cboDriver.SelectedValue == null || cboRoute.SelectedValue == null)
            {
                MessageBox.Show("Please select a Bus, Driver, and Route.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Schedule s = new Schedule
                {
                    ScheduleID = Convert.ToInt32(txtScheduleID.Text),
                    BusID = Convert.ToInt32(cboBus.SelectedValue),
                    DriverID = Convert.ToInt32(cboDriver.SelectedValue),
                    RouteID = Convert.ToInt32(cboRoute.SelectedValue),
                    DepartureTime = dtpDepartureTime.Value,
                    ArrivalTime = dtpArrivalTime.Value
                };
                if (_scheduleBLL.Update(s))
                {
                    MessageBox.Show("Schedule updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSchedules();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to update schedule.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtScheduleID.Text))
            {
                MessageBox.Show("Please select a schedule from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this schedule? Bookings for this schedule will also be deleted.",
                                "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(txtScheduleID.Text);
                    if (_scheduleBLL.Delete(id))
                    {
                        MessageBox.Show("Schedule deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadSchedules();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete schedule.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PerformSearch()
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                LoadSchedules();
                return;
            }
            try
            {
                DataTable dt = _scheduleBLL.Search(keyword);
                dgvSchedule.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PerformSearch();
            }
        }

        private void dgvSchedule_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSchedule.Rows[e.RowIndex];
                txtScheduleID.Text = row.Cells["ScheduleID"].Value.ToString();
                cboBus.SelectedValue = row.Cells["BusID"].Value;
                cboDriver.SelectedValue = row.Cells["DriverID"].Value;
                cboRoute.SelectedValue = row.Cells["RouteID"].Value;
                dtpDepartureTime.Value = Convert.ToDateTime(row.Cells["DepartureTime"].Value);
                dtpArrivalTime.Value = Convert.ToDateTime(row.Cells["ArrivalTime"].Value);
            }
            btnSave.Enabled = false;
        }
    }
}
