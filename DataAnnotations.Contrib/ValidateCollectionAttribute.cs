using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAnnotations.Contrib
{
	public class ValidateCollectionAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var collection = value as ICollection;

			if (collection == null)
				return ValidationResult.Success;

			foreach (var obj in collection)
			{
				var results = new List<ValidationResult>();
				var context = new ValidationContext(obj, null, null);

				Validator.TryValidateObject(obj, context, results, true);

				if (results.Count > 0)
					return results[0];
			}

			return ValidationResult.Success;
		}
	}
}
