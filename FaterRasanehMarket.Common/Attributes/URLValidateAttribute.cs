using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FaterRasanehMarket.Common.Attributes
{
    public class URLValidateAttribute : ValidationAttribute
    {
        public IEnumerable<string> Characters { get; }
        public URLValidateAttribute(params string[] character)
        {
            Characters = character;
        }
        public override bool IsValid(object value)
        {
            if (value != null)
                return !Characters.Any(value.ToString().Contains);
            return false;
        }
        public override string FormatErrorMessage(string name)
        {
            
            return $"{name} نباید شامل کاراکترهای غیر مجاز فضای خالی و ({String.Join(",", Characters).Replace(" ,","")}) باشد.";
        }
    }
}
