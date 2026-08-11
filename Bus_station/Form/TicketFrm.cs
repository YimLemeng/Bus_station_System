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
    public partial class TicketFrm : Form
    {
        private readonly UserAccount _currentUser;
        private readonly TicketBLL _ticketBLL = new TicketBLL();
        private readonly BookingBLL _bookingBLL = new BookingBLL();
        private readonly ScheduleBLL _scheduleBLL = new ScheduleBLL();
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
        public TicketFrm(UserAccount user)
        {
            InitializeComponent();
            _currentUser = user;
            ApplyRolePrivileges();
            LoadBookings();
            LoadTickets();
            ClearForm();
            cboBooking.DropDownStyle = ComboBoxStyle.DropDownList;
            txtTicketID.ReadOnly = true;
        }

        private void TicketFrm_Load(object sender, EventArgs e)
        {
            txtSearch.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtSearch.Width, txtSearch.Height, 25, 25));
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

        private void ApplyRolePrivileges()
        {
            btnDelete.Visible = (_currentUser.Role == "Admin");
        }

        private void LoadBookings()
        {
            try
            {
                DataTable dt = _bookingBLL.GetAllBooking();
                if (!dt.Columns.Contains("BookingDisplay"))
                {
                    dt.Columns.Add("BookingDisplay", typeof(string),
                        "'Booking #' + BookingID + ' - ' + CustomerName + ' (' + Departure + ' to ' + Destination + ')'");
                }

                cboBooking.DataSource = dt;
                cboBooking.DisplayMember = "BookingDisplay";
                cboBooking.ValueMember = "BookingID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load bookings dropdown: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTickets()
        {
            try
            {
                DataTable dt = _ticketBLL.GetAllTicket();
                dgvTicket.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load tickets: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtTicketID.Clear();
            if (cboBooking.Items.Count > 0) cboBooking.SelectedIndex = 0;
            dtpIssueDate.Value = DateTime.Now;
            txtPrice.Clear();
            txtSearch.Clear();
            btnSave.Enabled = true;
            AutoFillPrice();
        }

        private void AutoFillPrice()
        {
            if (cboBooking.SelectedValue != null && int.TryParse(cboBooking.SelectedValue.ToString(), out int bookingId))
            {
                try
                {
                    Booking bk = _bookingBLL.GetBookingById(bookingId);
                    if (bk != null)
                    {
                        Schedule sched = _scheduleBLL.GetScheduleById(bk.ScheduleID);
                        if (sched != null)
                        {
                            Route r = _routeBLL.GetRouteById(sched.RouteID);
                            if (r != null)
                            {
                                txtPrice.Text = r.Price.ToString("F2");
                            }
                        }
                    }
                }
                catch
                {
                    txtPrice.Text = "0.00";
                }
            }
            else
            {
                txtPrice.Text = "0.00";
            }
        }

        private void cboBooking_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoFillPrice();
        }

        private void dgvTicket_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvTicket.Rows[e.RowIndex];
                txtTicketID.Text = row.Cells["TicketID"].Value.ToString();
                cboBooking.SelectedValue = Convert.ToInt32(row.Cells["BookingID"].Value);
                dtpIssueDate.Value = Convert.ToDateTime(row.Cells["IssueDate"].Value);
                txtPrice.Text = Convert.ToDecimal(row.Cells["Price"].Value).ToString("F2");
            }
            btnSave.Enabled = false;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboBooking.SelectedValue == null)
            {
                MessageBox.Show("Please select a valid Booking.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price))
                {
                    MessageBox.Show("Please enter a valid price.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Ticket ticket = new Ticket
                {
                    TicketID = 0,
                    BookingID = Convert.ToInt32(cboBooking.SelectedValue),
                    IssueDate = dtpIssueDate.Value,
                    Price = price
                };
                if (_ticketBLL.Insert(ticket))
                {
                    MessageBox.Show("Ticket issued successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadTickets();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to issue the ticket.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTicketID.Text))
            {
                MessageBox.Show("Please select a ticket from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price))
                {
                    MessageBox.Show("Please enter a valid price.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Ticket ticket = new Ticket
                {
                    TicketID = Convert.ToInt32(txtTicketID.Text),
                    BookingID = Convert.ToInt32(cboBooking.SelectedValue),
                    IssueDate = dtpIssueDate.Value,
                    Price = price
                };
                if (_ticketBLL.Update(ticket))
                {
                    MessageBox.Show("Ticket updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadTickets();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to update ticket details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTicketID.Text))
            {
                MessageBox.Show("Please select a ticket from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this ticket?", "Delete Confirmation",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(txtTicketID.Text);
                    if (_ticketBLL.Delete(id))
                    {
                        MessageBox.Show("Ticket deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadTickets();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete ticket.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                LoadTickets();
                return;
            }
            try
            {
                DataTable dt = _ticketBLL.Search(keyword);
                dgvTicket.DataSource = dt;
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
