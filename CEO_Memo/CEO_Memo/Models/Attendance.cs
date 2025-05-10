using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CEO_Memo.Models
{
    [Table("attendance")]
    public class Attendance
    {
        [Key]
        public int AttendanceID { get; set; }

        public int? EmployeeID { get; set; }

        public int WorkDays { get; set; }

        public int AbsentDays { get; set; }

        public int LeaveDays { get; set; }

        public DateTime AttendanceMonth { get; set; }

        public DateTime CreatedAt { get; set; }
        public virtual Employee Employee { get; set; }
    }

}