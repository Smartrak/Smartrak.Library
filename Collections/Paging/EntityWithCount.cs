namespace Smartrak.Collections.Paging
{
	/// <summary>
	/// This is a wrapper for a SQL query which includes a count on each row. Using this style of query avoids the need for a second query.
	/// </summary>
	/// <typeparam name="T">The type of the entity</typeparam>
	public class EntityWithCount<T> where T : class
	{
		public T Entity { get; set; }
		public int Count { get; set; }
	}
}
