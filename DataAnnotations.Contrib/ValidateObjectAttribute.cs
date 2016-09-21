using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DataAnnotations.Contrib
{
	public class ValidateObjectAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var results = new List<ValidationResult>();
			var context = new ValidationContext(value, null, null);

			Validator.TryValidateObject(value, context, results, true);

			if (results.Count > 0)
				return results[0];

			return ValidationResult.Success;
		}
	}
}
