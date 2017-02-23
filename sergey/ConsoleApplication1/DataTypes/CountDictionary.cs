using System.Collections.Generic;

namespace ConsoleApplication1.DataTypes
{
	public class CountDictionary<TKey> : Dictionary<TKey, ulong>
	{
		public CountDictionary() { }

		public CountDictionary(IEnumerable<TKey> items)
		{
			foreach (var item in items)
				Increment(item);
		}

		public void Increment(TKey key)
		{
			ulong count;
			if (TryGetValue(key, out count))
				this[key] = count + 1;
			else
				this[key] = 1;
		}

		public void Decrement(TKey key)
		{
			ulong count;
			if (TryGetValue(key, out count))
				this[key] = count - 1;
			else
				this[key] = 1;
		}

		public ulong Stat(TKey key)
		{
			ulong count;
			return TryGetValue(key, out count) ? count : 0ul;
		}
	}
}
