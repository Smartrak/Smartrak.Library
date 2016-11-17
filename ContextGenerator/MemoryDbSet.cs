using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ContextGenerator
{
	public class MemoryDbSet<T> : IDbSet<T> where T : class
	{
		private readonly HashSet<T> _data;
		private readonly IQueryable _query;

		public MemoryDbSet()
		{
			_data = new HashSet<T>();
			_query = _data.AsQueryable();
		}

		public Type ElementType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Expression Expression => _query.Expression;

		public ObservableCollection<T> Local
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IQueryProvider Provider => _query.Provider;

		public T Add(T entity)
		{
			_data.Add(entity);
			return entity;
		}

		public T Attach(T entity)
		{
			_data.Add(entity);
			return entity;
		}

		public T Create()
		{
			throw new NotImplementedException();
		}

		public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
		{
			throw new NotImplementedException();
		}

		public T Find(params object[] keyValues)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		public T Remove(T entity)
		{
			_data.Remove(entity);
			return entity;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _data.GetEnumerator();
		}
	}
}