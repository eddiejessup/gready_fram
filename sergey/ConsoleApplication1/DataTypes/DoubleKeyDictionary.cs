using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;

namespace ConsoleApplication1.DataTypes
{
	public class DoubleKeyDictionary<TKey1, TKey2, TValue>
		: Dictionary<Pair<TKey1, TKey2>, TValue>
	{
		public DoubleKeyDictionary()
			: base(Pair<TKey1, TKey2>.Comparer)
		{
		}

		public TValue this[TKey1 key1, TKey2 key2]
		{
			get { return this[new Pair<TKey1, TKey2>(key1, key2)]; }
			set { this[new Pair<TKey1, TKey2>(key1, key2)] = value; }
		}

		public void Add(TKey1 key1, TKey2 key2, TValue val) => Add(new Pair<TKey1, TKey2>(key1, key2), val);

		public bool ContainsKey(TKey1 key1, TKey2 key2)
		{
			return ContainsKey(new Pair<TKey1, TKey2>(key1, key2));
		}

		public TValue GetOrAdd(TKey1 key1, TKey2 key2, Func<TValue> create)
		{
			return this.GetOrAdd(new Pair<TKey1, TKey2>(key1, key2), _ => create());
		}

		public TValue GetOrDefault(TKey1 key1, TKey2 key2)
		{
			return this.GetOrDefault(new Pair<TKey1, TKey2>(key1, key2));
		}

		public void AddRange(IEnumerable<TValue> seq, Func<TValue, TKey1> getKey1, Func<TValue, TKey2> getKey2)
		{
			foreach (var value in seq)
			{
				var key1 = getKey1(value);
				var key2 = getKey2(value);
				Add(new Pair<TKey1, TKey2>(key1, key2), value);
			}
		}

		public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue val)
		{
			return TryGetValue(new Pair<TKey1, TKey2>(key1, key2), out val);
		}
	}
}