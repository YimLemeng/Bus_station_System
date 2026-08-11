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
    public partial class CustomerFrm : Form
    {
        private readonly CustomerBLL _customerBLL = new CustomerBLL();
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        public CustomerFrm()
        {
            InitializeComponent();
            LoadCustomers();
            ClearForm();
            txtCustomerID.ReadOnly = true;
            cboGender.Items.AddRange(new string[] { "Male", "Female" });
            cboGender.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LoadCustomers()
        {
            try
            {
                DataTable dt = _customerBLL.GetAllCustomers();
                dgvCustomer.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load customer records: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtCustomerID.Clear();
            txtFullName.Clear();
            cboGender.SelectedIndex = -1;
            txtPhone.Clear();
            txtEmail.Clear();
            txtSearch.Clear();
            txtFullName.Focus();
            btnSave.Enabled = true;
        }

        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCustomer.Rows[e.RowIndex];
                txtCustomerID.Text = row.Cells["CustomerID"].Value.ToString();
                txtFullName.Text = row.Cells["FullName"].Value.ToString();
                cboGender.SelectedItem = row.Cells["Gender"].Value.ToString();
                txtPhone.Text = row.Cells["Phone"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value.ToString();
            }
            btnSave.Enabled = false; 
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(cboGender.SelectedItem == null) return; 
            try
            {
                Customer cust = new Customer
                {
                    CustomerID = 0,
                    Name = txtFullName.Text.Trim(),
                    Gender = cboGender.SelectedItem.ToString(),
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim()
                };
                if (_customerBLL.Insert(cust))
                {
                    MessageBox.Show("Customer registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to save customer details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerID.Text))
            {
                MessageBox.Show("Please select a customer from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Customer cust = new Customer
                {
                    CustomerID = Convert.ToInt32(txtCustomerID.Text),
                    Name = txtFullName.Text.Trim(),
                    Gender = cboGender.SelectedItem.ToString(),
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim()
                };
                if (_customerBLL.Update(cust))
                {
                    MessageBox.Show("Customer details updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to update customer details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerID.Text))
            {
                MessageBox.Show("Please select a customer from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this customer record? Doing so will delete related bookings.",
                                "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(txtCustomerID.Text);
                    if (_customerBLL.Delete(id))
                    {
                        MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCustomers();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the customer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }
        
        private void PerformSearch()
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                LoadCustomers();
                return;
            }
            try
            {
                DataTable dt = _customerBLL.Search(keyword);
                dgvCustomer.DataSource = dt;
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

        private void CustomerFrm_Load(object sender, EventArgs e)
        {
            txtSearch.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtSearch.Width, txtSearch.Height, 25, 25));
        }
    }
}
