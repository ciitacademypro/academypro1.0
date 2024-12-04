using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LmsModels.Course
{
    public class CourseModuleContentModel
    {
        public int CourseModuleContentId { get; set; }
        public int CourseModuleId { get; set; }
        public string ModuleName { get; set; }
        
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string ContentDescription { get; set; }
        public string ContentName { get; set; }
        public byte DurationInHrs { get; set; }
        public short ContentOrder { get; set; }
        public bool Status { get; set; }
        public string StatusLabel { get; set; }
        

    }
}