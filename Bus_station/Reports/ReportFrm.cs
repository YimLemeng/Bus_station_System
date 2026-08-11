using Bus_station.BLL;
using Bus_station.DAL;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bus_station
{
    public partial class ReportFrm : Form
    {
        private readonly PaymentBLL _paymentBLL = new PaymentBLL();
        private readonly TicketBLL _ticketBLL = new TicketBLL();
        private readonly BookingBLL _bookingBLL = new BookingBLL();
        public ReportFrm()
        {
            InitializeComponent();
            cboReportType.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void ReportFrm_Load(object sender, EventArgs e)
        {
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;
            cboReportType.Items.Clear();
            cboReportType.Items.AddRange(new string[] { "Revenue & Financial Report", "Passenger Ticket Receipt", "Passenger Trip Manifest" });
            cboReportType.SelectedIndex = 0;
            this.reportViewer1.RefreshReport();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime fromDate = dtpFromDate.Value.Date;
                DateTime toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1);
                string selectedReport = cboReportType.SelectedItem != null ? cboReportType.SelectedItem.ToString() : "";
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                reportViewer1.LocalReport.DataSources.Clear();
                if (selectedReport == "Revenue & Financial Report")
                {
                    DataTable dt = _paymentBLL.GetRevenueReport();
                    DataView dv = new DataView(dt);
                    if (dt.Columns.Contains("RevenueDate"))
                    {
                        dv.RowFilter = $"RevenueDate >= #{fromDate:yyyy-MM-dd}# AND RevenueDate <= #{toDate:yyyy-MM-dd HH:mm:ss}#";
                    }
                    reportViewer1.LocalReport.ReportEmbeddedResource = "Bus_station.Reports.RevenueReport.rdlc";
                    reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("RevenueDataSet", dv.ToTable()));
                }
                else if (selectedReport == "Passenger Ticket Receipt")
                {
                    DataTable dt = _ticketBLL.GetAllTicket();
                    DataView dv = new DataView(dt);
                    if (dt.Columns.Contains("IssueDate"))
                    {
                        dv.RowFilter = $"IssueDate >= #{fromDate:yyyy-MM-dd}# AND IssueDate <= #{toDate:yyyy-MM-dd HH:mm:ss}#";
                    }
                    reportViewer1.LocalReport.ReportEmbeddedResource = "Bus_station.Reports.TicketReceipt.rdlc";
                    reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TicketDataSet", dv.ToTable()));
                }
                else if (selectedReport == "Passenger Trip Manifest")
                {
                    DataTable dt = _bookingBLL.GetAllBooking();
                    DataView dv = new DataView(dt);
                    if (dt.Columns.Contains("BookingDate"))
                    {
                        dv.RowFilter = $"BookingDate >= #{fromDate:yyyy-MM-dd}# AND BookingDate <= #{toDate:yyyy-MM-dd HH:mm:ss}#";
                    }
                    reportViewer1.LocalReport.ReportEmbeddedResource = "Bus_station.Reports.PassengerManifest.rdlc";
                    reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("PassengerManifestDataSet", dv.ToTable()));
                }
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while generating the report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
