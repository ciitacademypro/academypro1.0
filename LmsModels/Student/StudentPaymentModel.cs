using System;
using System.ComponentModel.DataAnnotations;

namespace LmsModels.Student;

public class StudentPaymentModel
{
        public int StudentPaymentId { get; set; }

        public int StudentEnrollmentId { get; set; }

        public int InstallmentCount { get; set; }

        public float InstallmentAmount { get; set; }

        public int AmountPaid { get; set; }

        public DateOnly InstallmentDate { get; set; }

        public DateOnly? PaidDate { get; set; }

        public string? PaymentMode { get; set; }

        public string? TransactionId { get; set; }

        public string? Screenshot { get; set; }

        public bool Status { get; set; }
}
