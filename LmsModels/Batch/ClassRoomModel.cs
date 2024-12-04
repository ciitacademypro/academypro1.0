using System.ComponentModel.DataAnnotations;

namespace LmsModels.Batch;

public class ClassRoomModel
{
        public int ClassRoomId { get; set; }

        [Display(Name = "Batch Id")]
	    public int BranchId { get; set; }

	    [Display(Name = "Firm Id")]
	    public int FirmId { get; set; }

	    [Display(Name = "Class Room Name")]
        public string ClassRoomName { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; }
        
        public string? StatusLabel { get; set; }
}
