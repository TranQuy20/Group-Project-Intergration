using System.Collections.Generic;

namespace CEO_Memo.Models.ViewModels
{
    public class LeaveViolationViewModel
    {
        // Danh sách các nhân viên vi phạm nghỉ phép
        public List<Attendance> Violations { get; set; }
    }
}
