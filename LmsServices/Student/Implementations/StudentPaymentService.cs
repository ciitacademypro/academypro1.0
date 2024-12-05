using System;
using LmsEnv;
using LmsModels.Student;
using LmsServices.Common;
using LmsServices.Student.Interfaces;

namespace LmsServices.Student.Implementations;

public class StudentPaymentService : IStudentPaymentService
{
    private readonly string connString;

    public StudentPaymentService()
    {
			connString = DbConnect.DefaultConnection;
    }

		public void Create(StudentPaymentModel payment)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{
				new ("@Type", "INSERT"),
				new ("@StudentPaymentId", 0),
				new ("@StudentEnrollmentId",payment.StudentEnrollmentId),
				new ("@InstallmentCount", payment.InstallmentCount),
				new ("@InstallmentAmount", payment.InstallmentAmount),
				new ("@InstallmentDate", payment.InstallmentDate),
				new ("@Status", false),
							
			};
			QueryService.NonQuery("[sp_CreateUpdateDeleteRestore_StudentPayments]", parameters);
		}


        // exec sp_UpdateStudentPayment_StudentPayments 1, 0, 10000, 'cash','2222', 'default3.png',0
		public void PayInstallment( PayInstallmentModel pay)
		{
			var parameters = new List<KeyValuePair<string, object>>
			{

				new ("@StudentEnrollmentIdIn", pay.EnrollmentId),
				new ("@StudentPaymentId", 0),
				new ("@NewAmountPaid", pay.AmountPaid),
				new ("@PaymentMode", pay.PayMode),
				new ("@TransactionId", pay.TransactionId),
				new ("@Screenshot", pay.Screenshot),			
				new ("@CustomStatus", 0)

			};


			QueryService.NonQuery("[sp_UpdateStudentPayment_StudentPayments]", parameters);
		}



    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public List<StudentPaymentModel> GetAll()
    {
        throw new NotImplementedException();
    }

    public StudentPaymentModel GetById(int id)
    {
        throw new NotImplementedException();
    }

    public void Restore(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(StudentPaymentModel payment)
    {
        throw new NotImplementedException();
    }
}
