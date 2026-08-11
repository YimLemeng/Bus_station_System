using Bus_station.DAL;
using Bus_station.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bus_station.BLL
{
    public class PaymentBLL
    {
        private readonly PaymentDAL _paymentDAL = new PaymentDAL();

        public DataTable GetAllPayment() => _paymentDAL.GetAll();

        private void ValidatePayment(Payment pay)
        {
            if (pay.BookingID <= 0)
                throw new ArgumentException("Please select a valid Booking.");
            if (string.IsNullOrWhiteSpace(pay.PaymentMethod) ||
                    (pay.PaymentMethod != "Cash" && pay.PaymentMethod != "QR Payment" &&
                     pay.PaymentMethod != "Credit Card" && pay.PaymentMethod != "Bank Transfer"))
            {
                throw new ArgumentException("Payment Method must be 'Cash', 'QR Payment', 'Credit Card', or 'Bank Transfer'.");
            }
            if (pay.Amount < 0)
                throw new ArgumentException("Payment amount cannot be negative.");
            if (string.IsNullOrWhiteSpace(pay.Status) ||
                (pay.Status != "Paid" && pay.Status != "Pending" && pay.Status != "Refunded"))
            {
                throw new ArgumentException("Payment Status must be 'Paid', 'Pending', or 'Refunded'.");
            }
        }

        public bool InsertPayment(Payment pay)
        {
            ValidatePayment(pay);
            return _paymentDAL.Insert(pay);
        }
        public bool UpdatePayment(Payment pay)
        {
            if (pay.PaymentID <= 0)
                throw new ArgumentException("Please select a valid Payment to update.");
            ValidatePayment(pay);
            return _paymentDAL.Update(pay);
        }
        public bool DeletePayment(int paymentID)
        {
            if (paymentID <= 0)
                throw new ArgumentException("Please select a valid Payment to delete.");
            return _paymentDAL.Delete(paymentID);
        }
        public DataTable SearchPayment(string keyword) => _paymentDAL.Search(keyword);
        public DataTable GetRevenueReport() => _paymentDAL.GetRevenueReport();
    }
}
