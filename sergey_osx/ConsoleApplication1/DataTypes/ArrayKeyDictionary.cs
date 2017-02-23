using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1.DataTypes
{
	public class ArrayKeyDictionary<TKey, TValue> : Dictionary<TKey[], TValue>
	{
		public ArrayKeyDictionary() : base(ArrayKeyDictionaryKeyComparer<TKey>.Instance)
		{
		}

		public ArrayKeyDictionary(int capacity) : base(capacity, ArrayKeyDictionaryKeyComparer<TKey>.Instance)
		{
		}

		public ArrayKeyDictionary(IDictionary<TKey[], TValue> dictionary) : base(dictionary, ArrayKeyDictionaryKeyComparer<TKey>.Instance)
		{
		}
	}

	public class ArrayKeyDictionaryKeyComparer<TKey> : IEqualityComparer<TKey[]>
	{
		public static readonly ArrayKeyDictionaryKeyComparer<TKey> Instance 
			= new ArrayKeyDictionaryKeyComparer<TKey>();

		public bool Equals(TKey[] x, TKey[] y)
		{
			if (x == null) return y == null;
			if (y == null) return false;
			return x.SequenceEqual(y);
		}

		public int GetHashCode(TKey[] obj)
		{
			if (obj == null) return 0;
			if (obj.Length == 0) return 1;

			var result = obj[0].GetHashCode();

			for (var i = 1; i < obj.Length; i++)
				result = ((result << 5) + result) ^ obj[i].GetHashCode();

			return result;
		}
	}
}