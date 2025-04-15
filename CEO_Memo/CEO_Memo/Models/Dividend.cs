using System;

namespace CEO_Memo.Models
{
    public class Dividend
    {
        public int DividendID { get; set; }
        public int EmployeeID { get; set; }
        public decimal DividendAmount { get; set; }
        public DateTime DividendDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property (tuỳ chọn nếu bạn có quan hệ đến Employee)
        public virtual Employee Employee { get; set; }
    }
}
