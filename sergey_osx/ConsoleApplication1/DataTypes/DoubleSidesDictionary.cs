using ConsoleApplication1.Helpers;
using System.Collections;
using System.Collections.Generic;

namespace ConsoleApplication1.DataTypes
{
	public class DoubleSidesDictionary<TLeft, TRight> : IEnumerable<KeyValuePair<TLeft, TRight>>
	{
		private readonly Dictionary<TLeft, TRight> lefts = new Dictionary<TLeft, TRight>();
		private readonly Dictionary<TRight, TLeft> rights = new Dictionary<TRight, TLeft>();

		public int Count => lefts.Count;

		public void Add(TLeft left, TRight right)
		{
			lefts.Add(left, right);
			rights.Add(right, left);
		}

		public void AddRange(IEnumerable<KeyValuePair<TLeft, TRight>> pairs)
		{
			foreach (var pair in pairs)
				Add(pair.Key, pair.Value);
		}

		public TLeft GetLeft(TRight right)
		{
			return rights.Get(right);
		}

		public bool TryGetLeft(TRight right, out TLeft result)
		{
			return rights.TryGetValue(right, out result);
		}

		public TRight GetRight(TLeft left)
		{
			return lefts.Get(left);
		}

		public bool TryGetRight(TLeft left, out TRight result)
		{
			return lefts.TryGetValue(left, out result);
		}

		public IEnumerator<KeyValuePair<TLeft, TRight>> GetEnumerator()
		{
			return lefts.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Clear()
		{
			lefts.Clear();
			rights.Clear();
		}
	}
}