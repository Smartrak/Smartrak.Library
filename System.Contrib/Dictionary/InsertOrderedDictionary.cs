using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Contrib.Dictionary
{
	/// <summary>
	/// An implementation of IDictionary whose Keys, Values and KeyValuePairs can be enumerated in insert order.
	/// Provides additional add methods: Prepend(), InsertBefore(), InsertAfter().
	/// Provides additional properties: First, FirstKey, FirstValue, Last, LastKey, LastValue.
	/// </summary>
	/// <remarks>
	/// Dictionary access is backed by an internal Dictionary, while ordering is provided by an internal LinkedList.
	/// </remarks>
	public class InsertOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly IDictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _dictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
		private readonly LinkedList<KeyValuePair<TKey, TValue>> _list = new LinkedList<KeyValuePair<TKey, TValue>>();

		private KeyCollection<TKey> _keyCollection;
		private ValueCollection<TValue> _valueCollection;

		public TValue this[TKey key]
		{
			get
			{
				return _dictionary[key].Value.Value;
			}
			set
			{
				LinkedListNode<KeyValuePair<TKey, TValue>> node;
				if (_dictionary.TryGetValue(key, out node))
					node.Value = new KeyValuePair<TKey, TValue>(key, value);
				else
					Add(key, value);
			}
		}

		public ICollection<TKey> Keys
		{
			get { return _keyCollection ?? (_keyCollection = new KeyCollection<TKey>(this)); }
		}

		public ICollection<TValue> Values
		{
			get { return _valueCollection ?? (_valueCollection = new ValueCollection<TValue>(this)); }
		}

		public TKey FirstKey
		{
			get { return _list.First.Value.Key; }
		}

		public TValue FirstValue
		{
			get { return _list.First.Value.Value; }
		}

		public KeyValuePair<TKey, TValue> First
		{
			get { return new KeyValuePair<TKey, TValue>(FirstKey, FirstValue); }
		}

		public TKey LastKey
		{
			get { return _list.Last.Value.Key; }
		}

		public TValue LastValue
		{
			get { return _list.Last.Value.Value; }
		}

		public KeyValuePair<TKey, TValue> Last
		{
			get { return new KeyValuePair<TKey, TValue>(LastKey, LastValue); }
		}

		public int Count
		{
			get { return _dictionary.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public void Add(TKey key, TValue value)
		{
			var node = _list.AddLast(new KeyValuePair<TKey, TValue>(key, value));

			_dictionary.Add(key, node);
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Prepend(TKey key, TValue value)
		{
			var node = _list.AddFirst(new KeyValuePair<TKey, TValue>(key, value));

			_dictionary.Add(key, node);
		}

		public void InsertBefore(TKey beforeKey, TKey key, TValue value)
		{
			var beforeNode = _dictionary[beforeKey];
			var node = _list.AddBefore(beforeNode, new KeyValuePair<TKey, TValue>(key, value));

			_dictionary.Add(key, node);
		}

		public void InsertAfter(TKey afterKey, TKey key, TValue value)
		{
			var afterNode = _dictionary[afterKey];
			var node = _list.AddAfter(afterNode, new KeyValuePair<TKey, TValue>(key, value));

			_dictionary.Add(key, node);
		}

		public bool Remove(TKey key)
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> node;
			if (!_dictionary.TryGetValue(key, out node))
				return false;

			_dictionary.Remove(key);
			_list.Remove(node);
			return true;
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> node;
			if (!_dictionary.TryGetValue(item.Key, out node) || !Equals(node.Value.Value, item.Value))
				return false;

			_dictionary.Remove(item.Key);
			_list.Remove(node);
			return true;
		}

		public bool ContainsKey(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return _dictionary.ContainsKey(item.Key) && Equals(_dictionary[item.Key].Value.Value, item.Value);
		}

		public void Clear()
		{
			_list.Clear();
			_dictionary.Clear();
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			foreach (var kvp in _list)
				array[arrayIndex++] = new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> node;
			if (_dictionary.TryGetValue(key, out node))
			{
				value = node.Value.Value;
				return true;
			}

			value = default(TValue);
			return false;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private abstract class ReadonlyCollection<T> : ICollection<T>
		{
			public abstract int Count { get; }
			public abstract IEnumerator<T> GetEnumerator();
			public abstract bool Contains(T item);
			public abstract void CopyTo(T[] array, int arrayIndex);

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void Add(T item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public bool Remove(T item)
			{
				throw new NotSupportedException();
			}

			public bool IsReadOnly
			{
				get { return true; }
			}
		}

		private class KeyCollection<T> : ReadonlyCollection<T>
		{
			private readonly InsertOrderedDictionary<T, TValue> _dict;

			public override int Count
			{
				get { return _dict.Count; }
			}

			public KeyCollection(InsertOrderedDictionary<T, TValue> dict)
			{
				_dict = dict;
			}

			public override IEnumerator<T> GetEnumerator()
			{
				return _dict._list.Select(l => l.Key).GetEnumerator();
			}

			public override bool Contains(T item)
			{
				return _dict.ContainsKey(item);
			}

			public override void CopyTo(T[] array, int arrayIndex)
			{
				foreach (var kvp in _dict._list)
					array[arrayIndex++] = kvp.Key;
			}
		}

		private class ValueCollection<T> : ReadonlyCollection<T>
		{
			private readonly InsertOrderedDictionary<TKey, T> _dict;

			public override int Count
			{
				get { return _dict.Count; }
			}

			public ValueCollection(InsertOrderedDictionary<TKey, T> dict)
			{
				_dict = dict;
			}

			public override IEnumerator<T> GetEnumerator()
			{
				return _dict._list.Select(l => l.Value).GetEnumerator();
			}

			public override bool Contains(T item)
			{
				return _dict._list.Any(l => Equals(l.Value, item));
			}

			public override void CopyTo(T[] array, int arrayIndex)
			{
				foreach (var kvp in _dict._list)
					array[arrayIndex++] = kvp.Value;
			}
		}
	}
}