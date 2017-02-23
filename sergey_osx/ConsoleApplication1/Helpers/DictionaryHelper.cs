using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1.Helpers
{
	public static class DictionaryHelper
	{
		public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
		{
			TValue val;
			if (dict.TryGetValue(key, out val))
				return val;

			throw new KeyNotFoundException($"Key '{key}' not found");
		}

		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> create)
		{
			TValue val;
			if (dict.TryGetValue(key, out val))
				return val;

			val = create(key);

			dict.Add(key, val);

			return val;
		}

		public static TValue GetOr<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> orFactory)
		{
			TValue val;
			if (dict.TryGetValue(key, out val))
				return val;

			return orFactory(key);
		}

		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
		{
			TValue val;
			if (dict.TryGetValue(key, out val))
				return val;

			return default(TValue);
		}

		public static void AddOrUpdate<TKey, TValue>(
			this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> createFactory, Func<TValue, TValue> updateFactory)
		{
			TValue val;
			if (dict.TryGetValue(key, out val))
				dict[key] = updateFactory(val);
			else
				dict.Add(key, createFactory());
		}

		public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, bool> where)
		{
			var toRemove = dict.Where(where).ToArray();
			foreach (var kvp in toRemove)
				dict.Remove(kvp.Key);
		}

		public static void AddToList<TKey, TValue>(this IDictionary<TKey, List<TValue>> dict, TKey key, TValue val)
		{
			GetList(dict, key).Add(val);
		}

		public static void AddToList<TKey, TValue>(this IDictionary<TKey, List<TValue>> dict, TKey key, IEnumerable<TValue> val)
		{
			GetList(dict, key).AddRange(val);
		}

		public static List<TValue> GetList<TKey, TValue>(this IDictionary<TKey, List<TValue>> dict, TKey key)
		{
			List<TValue> list;
			if (!dict.TryGetValue(key, out list))
			{
				list = new List<TValue>();
				dict.Add(key, list);
			}
			return list;
		}

		public static TValue GetOrAddNew<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
			where TValue : new()
		{
			TValue val;
			if (!dict.TryGetValue(key, out val))
			{
				val = new TValue();
				dict.Add(key, val);
			}
			return val;
		}

		public static Dictionary<TKey, List<TValue>> GroupByToDictionary<TKey, TValue>(
			this IEnumerable<TValue> seq,
			Func<TValue, TKey> keySelector)
		{
			var dict = new Dictionary<TKey, List<TValue>>();

			foreach (var item in seq)
			{
				var key = keySelector(item);

				List<TValue> list;
				if (!dict.TryGetValue(key, out list))
				{
					list = new List<TValue>();
					dict.Add(key, list);
				}

				list.Add(item);
			}

			return dict;
		}

		public static IEnumerable<TValue> GetOrEmptySequence<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key)
		{
			List<TValue> list;
			return dict.TryGetValue(key, out list) ? list : Enumerable.Empty<TValue>();
		}

		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> seq)
		{
			return seq.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		public static TKey GetFirstKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<TValue, bool> condition)
		{
			foreach (var kvp in dict)
				if (condition(kvp.Value))
					return kvp.Key;
			throw new KeyNotFoundException();
		}
	}
}