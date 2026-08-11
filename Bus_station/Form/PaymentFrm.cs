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
    public partial class PaymentFrm : Form
    {
        private readonly PaymentBLL _paymentBLL = new PaymentBLL();
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
        public PaymentFrm()
        {
            InitializeComponent();
            LoadBookings();
            LoadPayments();
            ClearForm();
            txtPaymentID.ReadOnly = true;
            cboPaymentMethod.Items.AddRange(new string[] { "Cash", "QR Payment", "Credit Card", "Bank Transfer" });
            cboPaymentMethod.SelectedIndex = 0;
            cboStatus.Items.AddRange(new string[] {"Paid","Pending","Refunded"});
            cboStatus.SelectedIndex = 0;
            cboBooking.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            btnUpdate.Enabled = false;
        }

        private void PaymentFrm_Load(object sender, EventArgs e)
        {
            txtSearch.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtSearch.Width, txtSearch.Height, 25, 25));
        }

        private void LoadBookings()
        {
            try
            {
                DataTable dt = _bookingBLL.GetAllBooking();
                DataView dv = new DataView(dt);
                if (dt.Columns.Contains("PaymentStatus"))
                {
                    dv.RowFilter = "PaymentStatus IS NULL OR PaymentStatus <> 'Paid'";
                }

                DataTable dtUnpaid = dv.ToTable();
                if (!dtUnpaid.Columns.Contains("BookingDisplay"))
                {
                    dtUnpaid.Columns.Add("BookingDisplay", typeof(string),
                        "'Booking #' + BookingID + ' - ' + CustomerName + ' (' + Departure + ' to ' + Destination + ')'");
                }

                cboBooking.DataSource = dtUnpaid;
                cboBooking.DisplayMember = "BookingDisplay";
                cboBooking.ValueMember = "BookingID";
                cboBooking.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load bookings dropdown: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPayments()
        {
            try
            {
                DataTable dt = _paymentBLL.GetAllPayment();
                dgvPayment.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load payments: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtPaymentID.Clear();
            cboBooking.SelectedIndex = -1;
            if (cboPaymentMethod.Items.Count > 0) cboPaymentMethod.SelectedIndex = 0;
            if (cboStatus.Items.Count > 0) cboStatus.SelectedIndex = 0;
            txtAmount.Clear();
            dtpPaymentDate.Value = DateTime.Now;
            txtSearch.Clear();
            AutoFillAmount();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
        }

        private void AutoFillAmount()
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
                                txtAmount.Text = r.Price.ToString("F2");
                            }
                        }
                    }
                }
                catch
                {
                    txtAmount.Text = "0.00";
                }
            }
            else
            {
                txtAmount.Text = "0.00";
            }
        }

        private void cboBooking_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoFillAmount();
        }

        private void dgvPayment_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPayment.Rows[e.RowIndex];
                txtPaymentID.Text = row.Cells["PaymentID"].Value.ToString();
                cboBooking.SelectedValue = Convert.ToInt32(row.Cells["BookingID"].Value);
                cboPaymentMethod.SelectedItem = row.Cells["PaymentMethod"].Value.ToString();
                txtAmount.Text = Convert.ToDecimal(row.Cells["Amount"].Value).ToString("F2");
                dtpPaymentDate.Value = Convert.ToDateTime(row.Cells["PaymentDate"].Value);
                cboStatus.SelectedItem = row.Cells["Status"].Value.ToString();
            }
            btnSave.Enabled = false;
            btnUpdate.Enabled = true;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboBooking.SelectedIndex == -1 || cboBooking.SelectedValue == null)
            {
                MessageBox.Show("Please select a valid Booking.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                int selectedBookingId = Convert.ToInt32(cboBooking.SelectedValue);
                DataTable dtAllPayments = _paymentBLL.GetAllPayment();
                foreach (DataRow row in dtAllPayments.Rows)
                {
                    if (row["BookingID"] != DBNull.Value && Convert.ToInt32(row["BookingID"]) == selectedBookingId && row["Status"].ToString() == "Paid")
                    {
                        MessageBox.Show("This Booking has already been paid! Duplicate payments are not allowed.",
                                "Duplicate Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                if (!decimal.TryParse(txtAmount.Text.Trim(), out decimal amount))
                {
                    MessageBox.Show("Please enter a valid decimal payment amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Payment pay = new Payment
                {
                    PaymentID = 0,
                    BookingID = Convert.ToInt32(cboBooking.SelectedValue),
                    PaymentMethod = cboPaymentMethod.SelectedItem.ToString(),
                    Amount = amount,
                    PaymentDate = dtpPaymentDate.Value,
                    Status = cboStatus.SelectedItem.ToString()
                };

                if (_paymentBLL.InsertPayment(pay))
                {
                  
                    if (pay.Status == "Paid")
                    {
                        try
                        {
                            TicketBLL ticketBLL = new TicketBLL();
                            DataTable dtTicket = ticketBLL.GetByBookingId(pay.BookingID);

                            if (dtTicket == null || dtTicket.Rows.Count == 0)
                            {
                                Ticket t = new Ticket
                                {
                                    TicketID = 0,
                                    BookingID = pay.BookingID,
                                    IssueDate = DateTime.Now,
                                    Price = pay.Amount
                                };

                                ticketBLL.Insert(t); 
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ticket Auto Generation Error: " + ex.Message);
                        }
                    }

                    MessageBox.Show("Payment record saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPayments();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to save the payment details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPaymentID.Text))
            {
                MessageBox.Show("Please select a payment from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (!decimal.TryParse(txtAmount.Text.Trim(), out decimal amount))
                {
                    MessageBox.Show("Please enter a valid decimal payment amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Payment pay = new Payment
                {
                    PaymentID = Convert.ToInt32(txtPaymentID.Text),
                    BookingID = Convert.ToInt32(cboBooking.SelectedValue),
                    PaymentMethod = cboPaymentMethod.SelectedItem.ToString(),
                    Amount = amount,
                    PaymentDate = dtpPaymentDate.Value,
                    Status = cboStatus.SelectedItem.ToString()
                };
                if (_paymentBLL.UpdatePayment(pay))
                {
                    if (pay.Status == "Paid")
                    {
                        try
                        {
                            TicketBLL ticketBLL = new TicketBLL();
                            DataTable dtTicket = ticketBLL.GetByBookingId(pay.BookingID);
                            if (dtTicket == null || dtTicket.Rows.Count == 0)
                            {
                                Ticket t = new Ticket
                                {
                                    TicketID = 0,
                                    BookingID = pay.BookingID,
                                    IssueDate = DateTime.Now,
                                    Price = pay.Amount
                                };
                                ticketBLL.Insert(t);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ticket Error: " + ex.Message);
                        }

                        MessageBox.Show("Payment record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPayments();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update payment record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void PerformSearch()
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                LoadPayments();
                return;
            }
            try
            {
                DataTable dt = _paymentBLL.SearchPayment(keyword);
                dgvPayment.DataSource = dt;
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
