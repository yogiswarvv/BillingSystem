using BillingSystem.Data;
using BillingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Repositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<decimal> GetTotalPaidAmountByBillIdAsync(int billId);
        Task<bool> IsTransactionRefExistsAsync(string transactionRef);
    }

    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(HospitalDbContext context) : base(context)
        {
        }

        public async Task<decimal> GetTotalPaidAmountByBillIdAsync(int billId)
        {
            return await _context.Payments
                .Where(p => p.BillId == billId)
                .SumAsync(p => p.PaidAmount);
        }

        public async Task<bool> IsTransactionRefExistsAsync(string transactionRef)
        {
            if (string.IsNullOrEmpty(transactionRef)) return false;
            return await _context.Payments.AnyAsync(p => p.TransactionRef == transactionRef);
        }
    }
}
