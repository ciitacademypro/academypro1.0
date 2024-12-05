
namespace LmsModels.Student
{
    public class PayInstallmentModel
    {
        public int EnrollmentId { get; set; }
        public float AmountPaid { get; set; }
        public string PayMode { get; set; }
        public string TransactionId { get; set; }
        public string Screenshot { get; set; }
        
    }
}