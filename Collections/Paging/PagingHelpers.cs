using System;
using System.Linq;

namespace Smartrak.Collections.Paging
{
	public static class PagingHelpers
	{
		/// <summary>
		/// Gets a paged list of entities with the total appended to each row in the resultset. This is a faster way of doing things than using 2 seperate queries
		/// </summary>
		/// <typeparam name="T">The type of the entity, can be inferred by the type of queryable you pass</typeparam>
		/// <param name="entities">A queryable of entities</param>
		/// <param name="page">the 1 indexed page number you are interested in, cannot be zero or negative</param>
		/// <param name="pageSize">the size of the page you want, cannot be zero or negative</param>
		/// <returns>A queryable of the page of entities with counts appended.</returns>
		public static IQueryable<EntityWithCount<T>> GetPageWithTotal<T>(this IQueryable<T> entities, int page, int pageSize) where T : class
		{
			if (entities == null)
			{
				throw new ArgumentNullException("entities");
			}
			if (page < 1)
			{
				throw new ArgumentException("Must be positive", "page");
			}
			if (pageSize < 1)
			{
				throw new ArgumentException("Must be positive", "pageSize");
			}

			return entities
				.Select(e => new EntityWithCount<T> { Entity = e, Count = entities.Count() })
				.Skip(page - 1 * pageSize)
				.Take(pageSize);
		}
	}
}
