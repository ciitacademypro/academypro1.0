using System;

namespace LmsModels;

public class EnrollmentWithPaymentResult
{
    public int StudentEnrollmentId { get; set; }
    public string CourseName { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public int InstallmentCount { get; set; }
    public double InstallmentAmount { get; set; }
    public double AmountPaid { get; set; }
    public DateTime InstallmentDate { get; set; }
    public bool PaymentStatus { get; set; }  // True if fully paid
}

public class EnrollmentViewModel
{
    public int StudentEnrollmentId { get; set; }
    public string CourseName { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public List<StudentPaymentViewModel> Payments { get; set; }
}

public class StudentPaymentViewModel
{
    public int InstallmentCount { get; set; }
    public double InstallmentAmount { get; set; }
    public double AmountPaid { get; set; }
    public DateTime InstallmentDate { get; set; }
    public bool Status { get; set; }  // True if fully paid, False otherwise
}
