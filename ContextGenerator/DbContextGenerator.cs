using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Moq;

namespace ContextGenerator
{
	public static class DbContextGenerator
	{
		public static T Generate<T>() where T : class
		{
			return GenerateMock<T>().Object;
		}

		public static Mock<T> GenerateMock<T>() where T : class
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
				mockContext.SetupGet(lambdaExpression).Returns(Activator.CreateInstance(typeof(MemoryDbSet<>).MakeGenericType(iDbSetProperty.PropertyType.GenericTypeArguments)));
			}

			return mockContext;
		}
	}
}