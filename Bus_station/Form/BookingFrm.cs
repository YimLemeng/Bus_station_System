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
    public partial class BookingFrm : Form
    {
        private readonly UserAccount _currentUser;
        private readonly BookingBLL _bookingBLL = new BookingBLL();
        private readonly CustomerBLL _customerBLL = new CustomerBLL();
        private readonly ScheduleBLL _scheduleBLL = new ScheduleBLL();
        private readonly TicketBLL _ticketBLL = new TicketBLL();
        private readonly PaymentBLL _paymentBLL = new PaymentBLL();
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        public BookingFrm(UserAccount user)
        {
            InitializeComponent();
            _currentUser = user;
            ApplyRolePrivileges();
            LoadDropdowns();
            LoadBookings();
            ClearForm();
            txtBookingID.ReadOnly = true;
            cboCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSchedule.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboGender.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPaymentMethod.Items.AddRange(new string[] { "Cash", "QR Payment", "Credit Card", "Bank Transfer" });
            cboPaymentMethod.SelectedIndex = 0;
            cboStatus.Items.Clear();
            cboStatus.Items.AddRange(new string[] { "Confirmed", "Pending", "Cancelled" });
            cboStatus.SelectedIndex = 0;
            cboGender.Items.Clear();
            cboGender.Items.AddRange(new string[] { "Male", "Female", "Other" });
            cboGender.SelectedIndex = 0;
            btnUpdate.Enabled = false;
        }

        private void ApplyRolePrivileges()
        {
            btnDelete.Visible = (_currentUser != null && _currentUser.Role == "Admin");
        }

        private void LoadDropdowns()
        {
            try
            {
                DataTable dtCustomers = _customerBLL.GetAllCustomers();
                cboCustomer.DataSource = dtCustomers;
                cboCustomer.DisplayMember = "FullName";
                cboCustomer.ValueMember = "CustomerID";

                DataTable dtSchedules = _scheduleBLL.GetAllSchedule();
                // Create a description for each schedule
                dtSchedules.Columns.Add("ScheduleDetails", typeof(string),
                    "BusNumber + ' | ' + Departure + ' to ' + Destination + ' @ ' + DepartureTime");
                cboSchedule.DataSource = dtSchedules;
                cboSchedule.DisplayMember = "ScheduleDetails";
                cboSchedule.ValueMember = "ScheduleID";
                cboPaymentMethod.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load lookup dropdowns: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBookings()
        {
            try
            {
                DataTable dt = _bookingBLL.GetAllBooking();
                dgvBooking.DataSource = dt;
                string[] columnsToHide = {
                    "CustomerID", "ScheduleID", "CustomerEmail", "ArrivalTime",
                    "BusType", "DriverName", "TicketID", "PaymentID",
                    "PaymentMethod", "PaidAmount", "PaymentStatus", "BookingStatus"
                };
                foreach (string col in columnsToHide)
                {
                    if (dgvBooking.Columns.Contains(col))
                    {
                        dgvBooking.Columns[col].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load bookings: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtBookingID.Clear();
            if (cboCustomer.Items.Count > 0) cboCustomer.SelectedIndex = 0;
            if (cboSchedule.Items.Count > 0) cboSchedule.SelectedIndex = -1;
            txtSeatNumber.Clear();
            if (cboGender.Items.Count > 0) cboGender.SelectedIndex = 0;
            if (cboStatus.Items.Count > 0) cboStatus.SelectedIndex = 0;
            ChkPayNow.Checked = true;
            if (cboPaymentMethod.Items.Count > 0) cboPaymentMethod.SelectedIndex = 0;
            txtSearch.Clear();
            txtPhone.Clear();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            UpdateAvailableSeats();
        }

        private void UpdateAvailableSeats()
        {
            if (cboSchedule.SelectedValue == null)
            {
                lblAvailableSeat.Text = "Select schedule to check...";
                lblTotalAmount.Text = "$0.00";
                return;
            }

            int schedId = 0;
            if (cboSchedule.SelectedValue is int id)
            {
                schedId = id;
            }
            else if (cboSchedule.SelectedItem is DataRowView drv)
            {
                schedId = Convert.ToInt32(drv["ScheduleID"]);
            }
            else if (!int.TryParse(cboSchedule.SelectedValue.ToString(), out schedId))
            {
                lblAvailableSeat.Text = "Select schedule to check...";
                lblTotalAmount.Text = "$0.00";
                return;
            }
            if (schedId > 0)
            {
                try
                {
                    int available = _scheduleBLL.GetAvailableSeats(schedId);
                    lblAvailableSeat.Text = $"{available} seats available";
                    Schedule sched = _scheduleBLL.GetScheduleById(schedId);
                    if (sched != null)
                    {
                        RouteBLL routeBLL = new RouteBLL();
                        Route route = routeBLL.GetRouteById(sched.RouteID);
                        if (route != null)
                        {
                            lblTotalAmount.Text = $"${route.Price:N2}";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Debug Error: " + ex.Message);
                    lblAvailableSeat.Text = "Error loading capacity";
                    lblTotalAmount.Text = "$0.00";
                }
            }
        }

        private void cboSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAvailableSeats();
        }

        private void ChkPayNow_CheckedChanged(object sender, EventArgs e)
        {
            cboPaymentMethod.Enabled = ChkPayNow.Checked;
            if (ChkPayNow.Checked)
            {
                cboStatus.SelectedItem = "Confirmed";
            }
        }

        private void dgvBooking_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvBooking.Rows[e.RowIndex];
                txtBookingID.Text = row.Cells["BookingID"].Value.ToString();
                cboCustomer.SelectedValue = Convert.ToInt32(row.Cells["CustomerID"].Value);
                if (dgvBooking.Columns.Contains("CustomerPhone") && row.Cells["CustomerPhone"].Value != null)
                {
                    string phone = row.Cells["CustomerPhone"].Value.ToString();
                    if (phone == "0000000000")
                        txtPhone.Clear(); 
                    else
                        txtPhone.Text = phone; 
                }
                if (dgvBooking.Columns.Contains("Gender") && row.Cells["Gender"].Value != null)
                    cboGender.SelectedItem = row.Cells["Gender"].Value.ToString();

                cboSchedule.SelectedValue = Convert.ToInt32(row.Cells["ScheduleID"].Value);
                txtSeatNumber.Text = row.Cells["SeatNumber"].Value.ToString();
                if (row.Cells["BookingStatus"].Value != null)
                    cboStatus.SelectedItem = row.Cells["BookingStatus"].Value.ToString();
                if (dgvBooking.Columns.Contains("PaymentStatus") &&
                row.Cells["PaymentStatus"].Value != null &&
                row.Cells["PaymentStatus"].Value.ToString() == "Paid")
                {
                    ChkPayNow.Checked = true;
                }
                else
                {
                    ChkPayNow.Checked = false;
                }
                ChkPayNow.Enabled = true;
                cboPaymentMethod.Enabled = true;
                btnSave.Enabled = false;
                btnUpdate.Enabled = true;
            }
        }

        private void BookingFrm_Load(object sender, EventArgs e)
        {
            txtSearch.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtSearch.Width, txtSearch.Height, 25, 25));
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
            ChkPayNow.Enabled = true;
            cboPaymentMethod.Enabled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboCustomer.SelectedValue == null || cboSchedule.SelectedValue == null)
            {
                MessageBox.Show("Please select a Customer and a Schedule.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cboStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a Booking Status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int customerId = Convert.ToInt32(cboCustomer.SelectedValue);
                string phone = txtPhone.Text.Trim();
                string gender = cboGender.Text.Trim();
                if (string.IsNullOrWhiteSpace(gender)) gender = "Male";
                if (cboCustomer.Text.Contains("Walk-in") && !string.IsNullOrWhiteSpace(phone) && phone != "0000000000")
                {
                    CustomerBLL custBLL = new CustomerBLL();
                    Customer newWalkin = new Customer
                    {
                        CustomerID = 0,
                        Name = $"Walk-in ({phone})",
                        Gender = gender,
                        Phone = phone,
                        Email = "walkin@busstation.com"
                    };

                    int newCustId = custBLL.InsertAndGetId(newWalkin);
                    if (newCustId > 0)
                    {
                        customerId = newCustId; 
                    }
                }

                if (!int.TryParse(txtSeatNumber.Text.Trim(), out int seatNum))
                {
                    MessageBox.Show("Seat number must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Booking bk = new Booking
                {
                    BookingID = 0,
                    CustomerID = customerId,
                    ScheduleID = Convert.ToInt32(cboSchedule.SelectedValue),
                    BookingDate = DateTime.Now,
                    SeatNumber = seatNum,
                    Status = cboStatus.SelectedItem.ToString()
                };

                int newBookingId = _bookingBLL.InsertBooking(bk);
                if (newBookingId > 0)
                {
                    string successMsg = "Booking saved successfully!";
                    // If "Pay Now" is checked, automatically issue Ticket and Payment records
                    if (ChkPayNow.Checked && bk.Status == "Confirmed")
                    {
                        try
                        {
                            // Fetch price from selected schedule route price
                            Schedule sched = _scheduleBLL.GetScheduleById(bk.ScheduleID);
                            RouteBLL routeBLL = new RouteBLL();
                            Route route = routeBLL.GetRouteById(sched.RouteID);
                            decimal routePrice = route.Price;

                            Ticket ticket = new Ticket
                            {
                                TicketID = 0,
                                BookingID = newBookingId,
                                IssueDate = DateTime.Now,
                                Price = routePrice
                            };
                            _ticketBLL.Insert(ticket);

                            Payment payment = new Payment
                            {
                                PaymentID = 0,
                                BookingID = newBookingId,
                                PaymentMethod = cboPaymentMethod.SelectedItem.ToString(),
                                Amount = routePrice,
                                PaymentDate = DateTime.Now,
                                Status = "Paid"
                            };
                            _paymentBLL.InsertPayment(payment);
                            successMsg += "\nTicket and Payment processed successfully.";
                        }
                        catch (Exception ex)
                        {
                            successMsg += $"\nWarning: Booking saved, but Ticket/Payment auto-generation failed: {ex.Message}";
                        }
                    }
                    MessageBox.Show(successMsg, "Booking Confirmed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBookings();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to save the booking.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); ;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBookingID.Text))
            {
                MessageBox.Show("Please select a booking from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                int customerId = Convert.ToInt32(cboCustomer.SelectedValue);
                string phone = txtPhone.Text.Trim();
                string gender = cboGender.Text.Trim();
                if (string.IsNullOrWhiteSpace(gender)) gender = "Male";
                if (cboCustomer.Text.Contains("Walk-in") && !string.IsNullOrWhiteSpace(phone) && phone != "0000000000")
                {
                    CustomerBLL custBLL = new CustomerBLL();
                    Customer newWalkin = new Customer
                    {
                        CustomerID = 0,
                        Name = $"Walk-in ({phone})",
                        Gender = gender,
                        Phone = phone, 
                        Email = "walkin@busstation.com"
                    };
                    int newCustId = custBLL.InsertAndGetId(newWalkin);
                    if (newCustId > 0)
                    {
                        customerId = newCustId; 
                    }
                }

                if (!int.TryParse(txtSeatNumber.Text.Trim(), out int seatNum))
                {
                    MessageBox.Show("Seat number must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Booking bk = new Booking
                {
                    BookingID = Convert.ToInt32(txtBookingID.Text),
                    CustomerID = customerId,
                    ScheduleID = Convert.ToInt32(cboSchedule.SelectedValue),
                    BookingDate = DateTime.Now,
                    SeatNumber = seatNum,
                    Status = cboStatus.SelectedItem.ToString()
                };
                if (_bookingBLL.UpdateBooking(bk))
                {
                    MessageBox.Show("Booking updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBookings();
                    ClearForm();
                    ChkPayNow.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Failed to update booking.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBookingID.Text))
            {
                MessageBox.Show("Please select a route from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this route record? All linked schedules and bookings will be deleted.",
                                "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int bookingId = Convert.ToInt32(txtBookingID.Text);

                    if (_bookingBLL.DeleteBooking(bookingId))
                    {
                        MessageBox.Show("Booking deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadBookings();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete booking.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void PerformSearch()
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                LoadBookings();
                return;
            }
            try
            {
                DataTable dt = _bookingBLL.SearchBooking(keyword);
                dgvBooking.DataSource = dt;
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

        private void cboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCustomer.SelectedValue != null && int.TryParse(cboCustomer.SelectedValue.ToString(), out int custId))
            {
                try
                {
                    CustomerBLL custBLL = new CustomerBLL();
                    Customer cust = custBLL.GetCustomerById(custId);
                    if (cust != null)
                    {
                        if (cust.Name.Contains("Walk-in"))
                        {
                            txtPhone.Clear();
                            cboGender.SelectedIndex = 0; 
                        }
                        else
                        {
                            txtPhone.Text = cust.Phone;
                            cboGender.SelectedItem = cust.Gender; 
                        }
                    }
                }
                catch
                {
                    txtPhone.Clear();
                }
            }
            UpdateAvailableSeats();
        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboStatus.SelectedItem != null)
            {
                string selectedStatus = cboStatus.SelectedItem.ToString();
                if (selectedStatus == "Confirmed")
                {
                    ChkPayNow.Checked = true;
                }
                else if (selectedStatus == "Pending" || selectedStatus == "Cancelled")
                {
                    ChkPayNow.Checked = false;
                }
            }
        }
    }
}
