using System;
using LmsModels.Student;

namespace LmsServices.Student.Interfaces;

public interface IStudentPaymentService
{
		public void Create(StudentPaymentModel payment);
		public void Update(StudentPaymentModel payment);
		public void Delete(int id);
		public void Restore(int id);
		public StudentPaymentModel GetById(int id);
		public List<StudentPaymentModel> GetAll();
		public void PayInstallment( PayInstallmentModel pay);


}
