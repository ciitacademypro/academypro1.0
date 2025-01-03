﻿using LmsModels.Student;

namespace LmsServices.Student.Interfaces
{
   public interface IStudentService
    {
        public int RowCount();
        public int Create(StudentAddModel student);
        public int Update(StudentModel student);
        public int Delete(int id);
        public int Restore(int id);
        public int ToggleStatus(string id, bool  status = false);
        public StudentModel GetById(int id, int firmId);

        public List<StudentModel> GetAll(int firmId, bool? status = null);
		public bool ChangePassword(int studentId, string newPassword);
		public void UpdateProfilePhoto(int studentId, string profilePhoto);

        public Dictionary<string, string> CheckExist(string StudentCode);


    }
}

