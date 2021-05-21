using System;
using System.ComponentModel.DataAnnotations;

namespace MMORPG.Validation {

    public class DateValidation : ValidationAttribute {
        public override bool IsValid(object value) {
            var dateTime = DateTime.Parse(Convert.ToString(value) ?? string.Empty);
            var isValid = DateTime.UtcNow > dateTime;
            return isValid;
        }
    }

}