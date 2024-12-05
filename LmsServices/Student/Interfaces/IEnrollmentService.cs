using LmsModels;
using LmsModels.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmsServices.Student.Interface
{
    public  interface IEnrollmentService
    {
        public  int Create(StudentEnrollmentModel studentEnrollment);
        public void Update(StudentEnrollmentModel studentEnrollment);
        public void Delete(int id);
        public void Restore(int id);
        List<StudentEnrollmentModel> GetAll(int firmId, bool? status = null);
        public StudentEnrollmentModel GetById(int id, int firmId);
        public Task<List<EnrollmentViewModel>> GetEnrollmentWisePayments(int studentId);

    }
}
