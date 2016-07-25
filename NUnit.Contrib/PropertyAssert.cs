using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace NUnit.Contrib
{
	public static class PropertyAssert
	{
		/// <summary>
		/// Assert that the given objects are equal, by checking each public property.
		/// Optionally does a deep nested compare.
		/// </summary>
		public static void AreEqual<T>(T expected, T actual, bool deepCompare = false)
		{
			AssertPropertiesEqual(typeof(T), expected, actual, null, null, 0, deepCompare ? 16 : 1);
		}

		private static void AssertPropertiesEqual(Type type, object expected, object actual, string propertyPath, int? propertyIndex, int depth, int maxDepth)
		{
			if (depth >= maxDepth || IsValueType(type) || Equals(expected, null) || Equals(actual, null))
			{
				var propertyDescriptor = GetPropertyDescriptor(propertyPath, propertyIndex);
				Assert.AreEqual(expected, actual, propertyDescriptor);
				return;
			}

			// If the objects refer to the same instance in memory, then of course they are equal!
			if (ReferenceEquals(expected, actual))
				return;

			depth++;

			var expectedEnumerable = expected as IEnumerable;
			var actualEnumerable = actual as IEnumerable;

			var enumerableInterface = type
				.GetInterfaces()
				.SingleOrDefault(i =>
					i.IsGenericType &&
					i.GetGenericTypeDefinition() == typeof (IEnumerable<>) &&
					i.GenericTypeArguments.Length == 1);

			if (expectedEnumerable != null && actualEnumerable != null && enumerableInterface != null)
			{
				var enumerableType = enumerableInterface.GetGenericArguments().Single();
				var expectedArray = expectedEnumerable.Cast<object>().ToArray();
				var actualArray = actualEnumerable.Cast<object>().ToArray();

				if (expectedArray.Length != actualArray.Length)
				{
					var propertyDescriptor = GetPropertyDescriptor(propertyPath, propertyIndex);
					Assert.Fail($"{propertyDescriptor}{Environment.NewLine}  Expected length: {expectedArray.Length}{Environment.NewLine}  But was length:  {actualArray.Length}{Environment.NewLine}");
				}

				for (int i = 0; i < expectedArray.Length; i++)
				{
					var expectedArrValue = expectedArray[i];
					var actualArrValue = actualArray[i];
					var newPropertyPath = GetNewPropertyPath(propertyPath, propertyIndex, null);

					AssertPropertiesEqual(enumerableType, expectedArrValue, actualArrValue, newPropertyPath, i, depth, maxDepth);
				}

				return;
			}

			var properties = type
				.GetProperties(BindingFlags.Public | BindingFlags.Instance) // Get all public properties,
				.Where(p => 
					p.CanRead && // With a Getter,
					p.GetMethod.IsPublic && // Where the getter is public,
					!p.GetIndexParameters().Any() // And the property is not an indexer[]
				);

			foreach (var property in properties)
			{
				var expectedValue = property.GetValue(expected, null);
				var actualValue = property.GetValue(actual, null);
				var newPropertyPath = GetNewPropertyPath(propertyPath, propertyIndex, property.Name);

				// Skip self-referential recursive properties, which refer to the object itself (ie: GeoAPI Coordinate)
				if (property.PropertyType == type && ReferenceEquals(expected, expectedValue) && ReferenceEquals(actual, actualValue))
					continue;

				AssertPropertiesEqual(property.PropertyType, expectedValue, actualValue, newPropertyPath, null, depth, maxDepth);
			}
		}

		private static string GetNewPropertyPath(string currentPath, int? currentIndex, string newProperty)
		{
			var newPath = "";

			if (currentPath != null)
				newPath += currentPath;

			if (currentIndex.HasValue)
				newPath += "[" + currentIndex.Value + "]";

			if (newPath != "" && newProperty != null)
				newPath += ".";

			if (newProperty != null)
				newPath += newProperty;

			return newPath != "" ? newPath : null;
		}

		private static string GetPropertyDescriptor(string propertyPath, int? propertyIndex)
		{
			if (propertyPath == null)
			{
				return propertyIndex.HasValue ?
					$"Values differ at index [{propertyIndex.Value}]."
					: "Values are not equal.";
			}

			return propertyIndex.HasValue ?
				$"Values differ at property '{propertyPath}' at index [{propertyIndex.Value}]."
				: $"Values differ at property '{propertyPath}'.";
		}

		private static bool IsValueType(Type type)
		{
			return type.IsValueType || type == typeof (string);
		}
	}
}
