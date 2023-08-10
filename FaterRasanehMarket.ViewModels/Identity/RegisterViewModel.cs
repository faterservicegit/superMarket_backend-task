using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FaterRasanehMarket.ViewModels.Identity
{
   public class RegisterViewModel
    {
        [Required(ErrorMessage = "لطفا شماره موبایل خود را وارد کنید")]
        [Display(Name = "شماره موبایل")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0]{1})\)?[-. ]?([0-9]{3})?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "شماره موبایل نامعتبر است")] 
        public string PhoneNumber { get; set; }
        public string ConfirmCode { get; set; }
        public bool ForgotPassword { get; set; }
    }
}
