using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Moq;

namespace Smartrak.EFMockContext
{
	public static class ContextMocker
	{
		public static T CreateMockContext<T>(bool clearDownExistingData = false) where T : class
		{
			var mockContext = new Mock<T>();
			var parameter = Expression.Parameter(typeof(T));

			foreach (var iDbSetProperty in typeof(T).GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(IDbSet<>)))
			{
				var body = Expression.PropertyOrField(parameter, iDbSetProperty.Name);
				var lambdaExpression = Expression.Lambda<Func<T, object>>(body, parameter);
				mockContext.SetupProperty(lambdaExpression, Activator.CreateInstance(typeof(FakeDbSet.InMemoryDbSet<>).MakeGenericType(iDbSetProperty.PropertyType.GenericTypeArguments), args: clearDownExistingData));
			}

			return mockContext.Object;
		}
	}
}
