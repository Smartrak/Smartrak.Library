using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Moq;

namespace FakeDbSet.ContextGenerator
{
	public static class ContextGenerator
	{
		public static T Generate<T>(bool clearDownExistingData = false) where T : class
		{
			var mockContext = new Mock<T>();
			var parameter = Expression.Parameter(typeof(T));

			if (typeof(T).GetMethod("SaveChanges") != null)
			{
				var body = Expression.Call(parameter, typeof(T).GetMethod("SaveChanges"));
				var lambdaExpression = Expression.Lambda<Func<T, int>>(body, parameter);
				mockContext.Setup(lambdaExpression).Returns(0);
			}

			foreach (var iDbSetProperty in typeof(T).GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(IDbSet<>)))
			{
				var body = Expression.PropertyOrField(parameter, iDbSetProperty.Name);
				var lambdaExpression = Expression.Lambda<Func<T, object>>(body, parameter);
				mockContext.SetupGet(lambdaExpression).Returns(Activator.CreateInstance(typeof(FakeDbSet.InMemoryDbSet<>).MakeGenericType(iDbSetProperty.PropertyType.GenericTypeArguments), args: clearDownExistingData));
			}

			return mockContext.Object;
		}
	}
}
