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
    public partial class BusFrm : Form
    {
        private readonly UserAccount _currentUser;
        private readonly BusBLL _busBLL = new BusBLL();
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        public BusFrm(UserAccount user)
        {
            InitializeComponent();
            _currentUser = user;
            ApplyRolePrivileges();
            LoadBuses();
            ClearForm();
            txtBusID.ReadOnly = true;
            cboBusType.Items.AddRange(new string[] { "Sleeper", "Luxury", "Normal", "VIP Van", "Mini Bus" });
            cboStatus.Items.AddRange(new string[] { "Available", "Maintenance" });
            cboBusType.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void BusFrm_Load(object sender, EventArgs e)
        {
            txtSearch.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtSearch.Width, txtSearch.Height, 25, 25));
        }

        private void ApplyRolePrivileges()
        {
            btnDelete.Enabled = (_currentUser.Role == "Admin");
        }
        private void LoadBuses()
        {
            try
            {
                DataTable dt = _busBLL.GetAllBuses();
                dgvBus.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load bus records: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtBusID.Clear();
            txtBusNumber.Clear();
            cboBusType.SelectedIndex = -1;
            txtTotalSeat.Clear();
            cboStatus.SelectedIndex = -1;
            txtSearch.Clear();
            txtBusNumber.Focus();
            btnSave.Enabled = true;
        }

        private void dgvBus_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvBus.Rows[e.RowIndex];
                txtBusID.Text = row.Cells["BusID"].Value.ToString();
                txtBusNumber.Text = row.Cells["BusNumber"].Value.ToString();
                cboBusType.SelectedItem = row.Cells["BusType"].Value.ToString();
                txtTotalSeat.Text = row.Cells["TotalSeat"].Value.ToString();
                cboStatus.SelectedItem = row.Cells["Status"].Value.ToString();
            }
            btnSave.Enabled = false; 
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboBusType.SelectedItem == null || cboStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select both Bus Type and Status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (!int.TryParse(txtTotalSeat.Text.Trim(), out int seats))
                {
                    MessageBox.Show("Total seats must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Bus bus = new Bus
                {
                    BusID = 0,
                    BusNumber = txtBusNumber.Text.Trim(),
                    BusType = cboBusType.SelectedItem.ToString(),
                    TotalSeat = seats,
                    Status = cboStatus.SelectedItem.ToString()
                };
                if (_busBLL.InsertBus(bus))
                {
                    MessageBox.Show("Bus added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBuses();
                    ClearForm();
                }
                else MessageBox.Show("Failed to add bus.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving bus: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBusID.Text))
            {
                MessageBox.Show("Please select a bus from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!int.TryParse(txtTotalSeat.Text.Trim(), out int seats))
                {
                    MessageBox.Show("Total seats must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Bus bus = new Bus
                {
                    BusID = Convert.ToInt32(txtBusID.Text),
                    BusNumber = txtBusNumber.Text.Trim(),
                    BusType = cboBusType.SelectedItem.ToString(),
                    TotalSeat = seats,
                    Status = cboStatus.SelectedItem.ToString()
                };
                if (_busBLL.UpdateBus(bus))
                {
                    MessageBox.Show("Bus record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBuses();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to update the bus record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating bus: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBusID.Text))
            {
                MessageBox.Show("Please select a bus from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this bus record?", "Delete Confirmation",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(txtBusID.Text);
                    if (_busBLL.DeleteBus(id))
                    {
                        MessageBox.Show("Bus deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadBuses();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the bus.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting bus: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PerformSearch()
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                LoadBuses();
                return;
            }
            try
            {
                DataTable dt = _busBLL.SearchBus(keyword);
                dgvBus.DataSource = dt;
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
    }
}
