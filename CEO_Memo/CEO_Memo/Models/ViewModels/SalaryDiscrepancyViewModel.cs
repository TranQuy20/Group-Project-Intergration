namespace CEO_Memo.Models.ViewModels
{
    public class SalaryDiscrepancyViewModel
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public decimal CurrentSalary { get; set; }
        public decimal PreviousSalary { get; set; }
        public decimal SalaryDifference { get; set; }
        public string EmployeeEmail { get; set; }
    }
}
