using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEO_Memo.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

}