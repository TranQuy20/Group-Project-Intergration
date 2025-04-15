using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEO_Memo.Models.Payroll
{
    [Table("employees")] // ← phải trùng tên bảng trong MySQL
    public class EmployeePayroll
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // vì bạn tự gán EmployeeID
        public int EmployeeID { get; set; }

        [Required]
        public string FullName { get; set; }

        public int? DepartmentID { get; set; }
        public int? PositionID { get; set; }
        public string Status { get; set; }
    }
}
