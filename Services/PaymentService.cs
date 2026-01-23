using BillingSystem.Models;
using BillingSystem.Repositories;

namespace BillingSystem.Services
{
    public interface IPaymentService
    {
        Task<int> ProcessPaymentAsync(Payment payment);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> ProcessPaymentAsync(Payment payment)
        {
            // 1. Validate Bill Exists
            var bill = await _unitOfWork.Bills.GetByIdAsync(payment.BillId);
            if (bill == null) throw new Exception("Invalid Bill ID");

            // 2. Prevent Duplicates if TransactionRef provided
            if (!string.IsNullOrEmpty(payment.TransactionRef))
            {
                if (await _unitOfWork.Payments.IsTransactionRefExistsAsync(payment.TransactionRef))
                {
                    throw new Exception("Duplicate Transaction Reference.");
                }
            }

            // 3. Validate Amount
            var totalPaid = await _unitOfWork.Payments.GetTotalPaidAmountByBillIdAsync(payment.BillId);
            if (totalPaid + payment.PaidAmount > bill.FinalAmount)
            {
                throw new Exception("Payment exceeds bill amount.");
            }

            payment.PaidDate = DateTime.Now;
            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.CompleteAsync();

            // 4. Update Bill Status if fully paid
            var newTotalPaid = totalPaid + payment.PaidAmount;
            if (newTotalPaid >= bill.FinalAmount)
            {
                bill.Status = BillStatus.Paid;
                _unitOfWork.Bills.Update(bill);
                await _unitOfWork.CompleteAsync();
            }

            return payment.PaymentId;
        }
    }
}
