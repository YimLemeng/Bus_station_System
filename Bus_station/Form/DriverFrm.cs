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
    public partial class DriverFrm : Form
    {
        private readonly DriverBLL _driverBLL = new DriverBLL();
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
           int nLeftRect,
           int nTopRect,
           int nRightRect,
           int nBottomRect,
           int nWidthEllipse,
           int nHeightEllipse
       );
        public DriverFrm()
        {
            InitializeComponent();
            txtDriverID.ReadOnly = true;
            LoadDrivers();
            ClearForm();
        }

        private void LoadDrivers()
        {
            try
            {
                DataTable dt = _driverBLL.GetAllDriver();
                dgvDriver.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load driver records: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtDriverID.Clear();
            txtDriverName.Clear();
            txtPhone.Clear();
            txtLicenseNumber.Clear();
            txtExperience.Clear();
            txtSearch.Clear();
            txtDriverName.Focus();
            btnSave.Enabled = true;
        }

        private void dgvDriver_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDriver.Rows[e.RowIndex];
                txtDriverID.Text = row.Cells["DriverID"].Value.ToString();
                txtDriverName.Text = row.Cells["DriverName"].Value.ToString();
                txtPhone.Text = row.Cells["Phone"].Value.ToString();
                txtLicenseNumber.Text = row.Cells["LicenseNumber"].Value.ToString();
                txtExperience.Text = row.Cells["Experience"].Value.ToString();
            }
            btnSave.Enabled = false;

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtExperience.Text.Trim(), out int exp))
                {
                    MessageBox.Show("Experience must be a valid number of years.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Driver drv = new Driver
                {
                    DriverID = 0,
                    DriverName = txtDriverName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    LicenseNumber = txtLicenseNumber.Text.Trim(),
                    Experience = exp
                };
                if (_driverBLL.Insert(drv))
                {
                    MessageBox.Show("Driver added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDrivers();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to add the driver.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving driver: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDriverID.Text))
            {
                MessageBox.Show("Please select a driver from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try 
            {
                if (!int.TryParse(txtExperience.Text.Trim(), out int exp))
                {
                    MessageBox.Show("Experience must be a valid number of years.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Driver drv = new Driver
                {
                    DriverID = Convert.ToInt32(txtDriverID.Text),
                    DriverName = txtDriverName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    LicenseNumber = txtLicenseNumber.Text.Trim(),
                    Experience = exp
                };
                if (_driverBLL.Update(drv))
                {
                    MessageBox.Show("Driver record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDrivers();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to update the driver record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating driver: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDriverID.Text))
            {
                MessageBox.Show("Please select a driver from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this driver record?", "Delete Confirmation",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(txtDriverID.Text);
                    if (_driverBLL.Delete(id))
                    {
                        MessageBox.Show("Driver deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDrivers();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the driver.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting driver: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } 
        }

        private void DriverFrm_Load(object sender, EventArgs e)
        {
            txtSearch.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtSearch.Width, txtSearch.Height, 25, 25));
        }

        private void txtExperience_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void PerformSearch()
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                LoadDrivers();
                return;
            }
            try
            {
                DataTable dt = _driverBLL.Search(keyword);
                dgvDriver.DataSource = dt;
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
    }
}
