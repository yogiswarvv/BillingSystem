namespace BillingSystem.Models
{
    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum InsuranceCoverageType
    {
        FullBill = 1,
        OptionalServicesOnly = 2,
        AdmitFeeOnly = 3,
        ConsultationFeeOnly = 4
    }

    public enum PaymentMode
    {
        Cash = 1,
        Card = 2,
        UPI = 3,
        Online = 4
    }

    public enum BillStatus
    {
        Unpaid = 1,
        Paid = 2
    }

    public enum BillItemType
    {
        Consultation = 1,
        Service = 2,
        Admission = 3,
        Tax = 4,
        Discount = 5,
        Insurance = 6
    }
}
