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
    public partial class EmployeeFrm : Form
    {
        private readonly UserAccount _currentUser;
        private readonly EmployeeBLL _employeeBLL = new EmployeeBLL();
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        public EmployeeFrm(UserAccount user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadEmployees();
            ClearForm();
            txtEmployeeID.ReadOnly = true;
            cboGender.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void EmployeeFrm_Load(object sender, EventArgs e)
        {
            txtSearch.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtSearch.Width, txtSearch.Height, 25, 25));
            
        }

        private void LoadEmployees()
        {
            try
            {
                dgvEmployee.DataSource = _employeeBLL.GetAllEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load employee records: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtEmployeeID.Clear();
            txtFullName.Clear();
            cboGender.SelectedIndex = -1;
            dtpDob.Value = DateTime.Now.AddYears(-25);
            txtPhone.Clear();
            txtEmail.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtPosition.Clear();
            txtSalary.Clear();
            txtSearch.Clear();
            txtFullName.Focus();
            btnSave.Enabled = true;
        }

        private void dgvEmployee_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvEmployee.Rows[e.RowIndex];
                txtEmployeeID.Text = row.Cells["EmployeeID"].Value.ToString();
                txtFullName.Text = row.Cells["FullName"].Value.ToString();
                cboGender.SelectedItem = row.Cells["Gender"].Value.ToString();
                dtpDob.Value = Convert.ToDateTime(row.Cells["DOB"].Value);
                txtPhone.Text = row.Cells["Phone"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value.ToString();
                txtAddress.Text = row.Cells["Address"].Value.ToString();
                txtPosition.Text = row.Cells["Position"].Value.ToString();
                txtSalary.Text = Convert.ToDecimal(row.Cells["Salary"].Value).ToString("F2");
            }
            btnSave.Enabled = false;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboGender.SelectedItem == null) return;
            try
            {
                if (!decimal.TryParse(txtSalary.Text.Trim(), out decimal sal))
                {
                    MessageBox.Show("Salary must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Employee emp = new Employee
                {
                    EmployeeID = 0,
                    FullName = txtFullName.Text.Trim(),
                    Gender = cboGender.SelectedItem.ToString(),
                    DOB = dtpDob.Value,
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Position = txtPosition.Text.Trim(),
                    Salary = sal
                };
                if (_employeeBLL.Insert(emp))
                {
                    MessageBox.Show("Employee registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEmployees();
                    ClearForm();
                }
                else MessageBox.Show("Failed to register employee.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save employee record: {ex.Message}");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
            {
                MessageBox.Show("Please select an employee from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!decimal.TryParse(txtSalary.Text.Trim(), out decimal sal))
                {
                    MessageBox.Show("Salary must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Employee emp = new Employee
                {
                    EmployeeID = Convert.ToInt32(txtEmployeeID.Text),
                    FullName = txtFullName.Text.Trim(),
                    Gender = cboGender.SelectedItem.ToString(),
                    DOB = dtpDob.Value,
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Position = txtPosition.Text.Trim(),
                    Salary = sal
                };
                if (_employeeBLL.Update(emp))
                {
                    MessageBox.Show("Employee details updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEmployees();
                    ClearForm();
                }
                else MessageBox.Show("Failed to update employee details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update employee record: {ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
            {
                MessageBox.Show("Please select an employee from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this employee account?", "Delete Confirmation",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(txtEmployeeID.Text);

                    // Prevent active admin self-deletion
                    if (id == _currentUser.EmployeeID)
                    {
                        MessageBox.Show("Security Violation: You cannot delete your own active administrator account.",
                                        "Operation Terminated", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (_employeeBLL.Delete(id))
                    {
                        MessageBox.Show("Employee deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadEmployees();
                        ClearForm();
                    }
                    else MessageBox.Show("Failed to delete employee.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete employee record: {ex.Message}");
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                LoadEmployees();
                return;
            }
            try
            {
                DataTable dt = _employeeBLL.Searching(keyword);
                dgvEmployee.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
