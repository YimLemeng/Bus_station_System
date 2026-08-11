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
using static System.Net.Mime.MediaTypeNames;

namespace Bus_station
{
    public partial class RouteFrm : Form
    {
        private readonly UserAccount _currentUser;
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
        public RouteFrm(UserAccount user)
        {
            InitializeComponent();
            txtRouteID.ReadOnly = true;
            _currentUser = user;
            ApplyRolePrivileges();
            LoadRoutes();
            ClearForm();

        }

        private void RouteFrm_Load(object sender, EventArgs e)
        {
            txtSearch.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtSearch.Width, txtSearch.Height, 25, 25));
        }

        private void ApplyRolePrivileges()
        {
            btnDelete.Enabled = (_currentUser.Role == "Admin");
        }
        private void LoadRoutes()
        {
            try
            {
                DataTable dt = _routeBLL.GetAllRoute();
                dgvRoute.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load route records: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtRouteID.Clear();
            txtDeparture.Clear();
            txtDestination.Clear();
            txtDistance.Clear();
            txtPrice.Clear();
            txtSearch.Clear();
            btnSave.Enabled = true;
            txtDeparture.Focus();
        }

        private void dgvRoute_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvRoute.Rows[e.RowIndex];
                txtRouteID.Text = row.Cells["RouteID"].Value.ToString();
                txtDeparture.Text = row.Cells["Departure"].Value.ToString();
                txtDestination.Text = row.Cells["Destination"].Value.ToString();
                txtDistance.Text = row.Cells["Distance"].Value.ToString();
                txtPrice.Text = row.Cells["Price"].Value.ToString();
            }
            btnSave.Enabled = false;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDeparture.Text) || string.IsNullOrWhiteSpace(txtDestination.Text) || string.IsNullOrWhiteSpace(txtDistance.Text) || string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtDeparture.Text.Trim().Equals(txtDestination.Text.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Departure and Destination cannot be the same location.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (!decimal.TryParse(txtDistance.Text.Trim(), out decimal dist))
                {
                    MessageBox.Show("Distance must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price))
                {
                    MessageBox.Show("Price must be a valid currency amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Route route = new Route
                {
                    RouteID = 0,
                    Departure = txtDeparture.Text.Trim(),
                    Destination = txtDestination.Text.Trim(),
                    Distance = dist,
                    Price = price
                };
                if (_routeBLL.Insert(route))
                {
                    MessageBox.Show("Route added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRoutes();
                    ClearForm();
                }
                else MessageBox.Show("Failed to add route.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRouteID.Text))
            {
                MessageBox.Show("Please select a route from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (!decimal.TryParse(txtDistance.Text.Trim(), out decimal dist))
                {
                    MessageBox.Show("Distance must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price))
                {
                    MessageBox.Show("Price must be a valid currency amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Route route = new Route
                {
                    RouteID = Convert.ToInt32(txtRouteID.Text),
                    Departure = txtDeparture.Text.Trim(),
                    Destination = txtDestination.Text.Trim(),
                    Distance = dist,
                    Price = price
                };
                if (_routeBLL.Update(route))
                {
                    MessageBox.Show("Route updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRoutes();
                    ClearForm();
                }
                else MessageBox.Show("Failed to update route.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRouteID.Text))
            {
                MessageBox.Show("Please select a route from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this route record? All linked schedules and bookings will be deleted.",
                                "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(txtRouteID.Text);
                    if (_routeBLL.Delete(id))
                    {
                        MessageBox.Show("Route deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRoutes();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete route.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                LoadRoutes();
                return;
            }
            try
            {
                DataTable dt = _routeBLL.Search(keyword);
                dgvRoute.DataSource = dt;
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

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true; 
            }
            if (e.KeyChar == '.' && txt != null && txt.Text.Contains("."))
            {
                e.Handled = true; 
            }
        }
    }
}
