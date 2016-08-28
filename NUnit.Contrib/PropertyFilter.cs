using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NUnit.Contrib
{
	public class PropertyFilter : IEnumerable<PropertyInfo>
	{
		private readonly IDictionary<Type, PropertyInfo[]> _filteredPropertiesByType = new Dictionary<Type, PropertyInfo[]>();
		private readonly IDictionary<Type, PropertyInfo[]> _allPropertiesByType = new Dictionary<Type, PropertyInfo[]>();

		/// <summary>
		/// Add the specified properties to the filter for the given type
		/// </summary>
		/// <returns>Returns 'this' for chainability</returns>
		/// <example>
		/// Specify a single property directly:
		/// <code>
		/// AddFilter{MyClass}(t => t.PropertyA);
		/// </code>
		/// </example>
		/// <example>
		/// Specify a collection of properties via an anonymous object:
		/// <code>
		/// AddFilter{MyClass}(t => new
		/// {
		///		t.PropertyA,
		///		t.PropertyB
		/// });
		/// </code>
		/// </example>
		/// <example>
		/// Specify a nested of property:
		/// <code>
		/// AddFilter{MyClass}(t => t.PropertyB.NestedProperty);
		/// </code>
		/// </example>
		public PropertyFilter AddFilter<T>(Expression<Func<T, dynamic>> propertySelector)
		{
			// Supports direct map: AddMap<MyClass>(a => a.PropA)
			if (propertySelector.Body is UnaryExpression)
			{
				var props = GetReferencedProperties((MemberExpression) ((UnaryExpression) propertySelector.Body).Operand);

				AddProperties(props);
			}
			// Support anon object maps: AddMap<MyClass>(a => new { a.PropA, a.PropB.PropC });
			else
			{
				var props = ((NewExpression) propertySelector.Body).Arguments
					.Cast<MemberExpression>()
					.SelectMany(GetReferencedProperties)
					.Distinct();

				AddProperties(props);
			}

			return this;
		}

		public IEnumerable<PropertyInfo> GetFilteredProperties(Type type)
		{
			PropertyInfo[] properties;
			return _filteredPropertiesByType.TryGetValue(type, out properties) ? properties : Enumerable.Empty<PropertyInfo>();
		}

		public IEnumerable<PropertyInfo> GetAllProperties(Type type)
		{
			PropertyInfo[] properties;
			if (!_allPropertiesByType.TryGetValue(type, out properties))
			{
				properties = type
					.GetProperties(BindingFlags.Public | BindingFlags.Instance) // Get all public properties,
					.Where(p =>
						p.CanRead && // With a Getter,
						p.GetMethod.IsPublic && // Where the getter is public,
						!p.GetIndexParameters().Any() // And the property is not an indexer[]
					)
					.ToArray();

				_allPropertiesByType.Add(type, properties);
			};

			return properties;
		}

		public IEnumerable<PropertyInfo> GetFilteredOrAllProperties(Type type)
		{
			PropertyInfo[] properties;
			return _filteredPropertiesByType.TryGetValue(type, out properties) ? properties : GetAllProperties(type);
		}

		private void AddProperties(IEnumerable<PropertyInfo> properties)
		{
			foreach (var typeProperties in properties.GroupBy(p => p.DeclaringType))
				_filteredPropertiesByType.Add(typeProperties.Key, typeProperties.ToArray());
		}

		private IEnumerable<PropertyInfo> GetReferencedProperties(MemberExpression expr)
		{
			var propertyInfo = expr.Member as PropertyInfo;
			if (propertyInfo != null)
				yield return propertyInfo;

			var outerMemberExpression = expr.Expression as MemberExpression;
			if (outerMemberExpression == null)
				yield break;

			// Recurse to handle nested property references
			foreach (var property in GetReferencedProperties(outerMemberExpression))
				yield return property;
		}

		public IEnumerator<PropertyInfo> GetEnumerator()
		{
			return _filteredPropertiesByType.Values.SelectMany(t => t).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}