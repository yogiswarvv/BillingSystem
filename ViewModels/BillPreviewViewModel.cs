namespace BillingSystem.ViewModels
{
    public class BillPreviewViewModel
    {
        public decimal ConsultationFee { get; set; }
        public decimal ServicesTotal { get; set; }
        public decimal PharmacyTotal { get; set; }
        public decimal AdmitAmount { get; set; }
        
        public decimal DiscountAmount { get; set; } // High value discount
        public decimal SeniorDiscount { get; set; }
        
        public decimal InsuranceDeduction { get; set; }
        
        public decimal TaxAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }
}
