using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ContextGenerator
{
	public class MemoryDbSet<T> : IDbSet<T> where T : class
	{
		private readonly HashSet<T> _data;
		private readonly IQueryable _query;

		public MemoryDbSet(bool async)
		{
			_data = new HashSet<T>();
			if (async)
				_query = new MemoryAsyncQueryable<T>(_data.AsQueryable());
			else
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

	internal class MemoryAsyncQueryable<T> : IQueryable
	{
		private readonly IQueryable<T> _queryable;

		public MemoryAsyncQueryable(IQueryable<T> queryable)
		{
			_queryable = queryable;
			Provider = new MemoryAsyncQueryProvider<T>(queryable.Provider);
		}

		public IEnumerator GetEnumerator()
		{
			return _queryable.GetEnumerator();
		}

		public Expression Expression => _queryable.Expression;

		public Type ElementType => _queryable.ElementType;

		public IQueryProvider Provider { get; }
	}

	internal class MemoryAsyncQueryProvider<T> : IDbAsyncQueryProvider
	{
		private readonly IQueryProvider _provider;

		public MemoryAsyncQueryProvider(IQueryProvider provider)
		{
			_provider = provider;
		}

		public IQueryable CreateQuery(Expression expression)
		{
			return new MemoryAsyncEnumerable<T>(expression);
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new MemoryAsyncEnumerable<TElement>(expression);
		}

		public object Execute(Expression expression)
		{
			return _provider.Execute(expression);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return _provider.Execute<TResult>(expression);
		}

		public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
		{
			return Task.FromResult(Execute(expression));
		}

		public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
		{
			return Task.FromResult(Execute<TResult>(expression));
		}
	}

	internal class MemoryAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
	{
		public MemoryAsyncEnumerable(IEnumerable<T> enumerable)
			: base(enumerable)
		{
		}

		public MemoryAsyncEnumerable(Expression expression)
			: base(expression)
		{
		}

		public IDbAsyncEnumerator<T> GetAsyncEnumerator()
		{
			return new MemoryAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
		}

		IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
		{
			return GetAsyncEnumerator();
		}

		IQueryProvider IQueryable.Provider => new MemoryAsyncQueryProvider<T>(this);
	}

	internal class MemoryAsyncEnumerator<T> : IDbAsyncEnumerator<T>
	{
		private readonly IEnumerator<T> _inner;

		public MemoryAsyncEnumerator(IEnumerator<T> inner)
		{
			_inner = inner;
		}

		public void Dispose()
		{
			_inner.Dispose();
		}

		public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
		{
			return Task.FromResult(_inner.MoveNext());
		}

		public T Current => _inner.Current;

		object IDbAsyncEnumerator.Current => Current;
	}
}