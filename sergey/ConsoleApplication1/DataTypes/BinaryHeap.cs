using System;
using System.Collections;
using System.Collections.Generic;

namespace ConsoleApplication1.DataTypes
{
	public static class BinaryHeap
	{
		public static BinaryHeap<int> CreateForInt()
		{
			return new BinaryHeap<int>(Comparer<int>.Default.Compare);
		}
	}

	public class BinaryHeap<T> : IEnumerable<T>
	{
		private readonly Comparison<T> comparisonDelegate;

		private readonly bool needNotifyIndexChange;
		private readonly Action<T, int> notifyIndexChange;

		public BinaryHeap(Comparison<T> comparisonDelegate)
		{
			this.comparisonDelegate = comparisonDelegate;
		}

		public BinaryHeap(Comparison<T> comparsionDelegate, Action<T, int> notifyIndexChange)
			: this(comparsionDelegate)
		{
			this.notifyIndexChange = notifyIndexChange;
			needNotifyIndexChange = true;
		}

		private readonly List<T> array = new List<T>();

		public T Min { get { return array[0]; } }

		public T this[int indexInHeap] { get { return array[indexInHeap]; } }

		public int Count { get { return array.Count; } }

		public void Clear()
		{
			array.Clear();
		}

		public void FillRaw(IEnumerable<T> elements)
		{
			array.Clear();
			array.AddRange(elements);

			if (!needNotifyIndexChange) return;
			for (var i = 0; i < array.Count; i++)
				notifyIndexChange(array[i], i);
		}

		public void BubbleUp(int i)
		{
			var k = array[i];

			while (true)
			{
				var pi = (i - 1) / 2;

				if (pi == i || comparisonDelegate(array[pi], k) <= 0)
					break;

				Swap(i, pi);

				i = pi;
			}
		}

		public void Insert(T k)
		{
			var i = array.Count;
			array.Add(k);

			if (needNotifyIndexChange)
				notifyIndexChange(k, i);

			BubbleUp(i);
		}

		public void DeleteMin()
		{
			Remove(0);
		}

		public void Remove(int indexInHeap)
		{
			var i = indexInHeap;

			if (needNotifyIndexChange)
				notifyIndexChange(array[i], -1);

			var k = array[array.Count - 1];
			array[i] = k;
			array.RemoveAt(array.Count - 1);

			if (needNotifyIndexChange && array.Count > 0)
				notifyIndexChange(k, i);

			while (true)
			{
				var ci = 2 * i + 1;

				if (ci >= array.Count)
					return;

				var left = array[ci];

				if (ci == array.Count - 1)
				{
					if (comparisonDelegate(left, k) < 0)
						Swap(i, ci);
					return;
				}

				var right = array[ci + 1];

				var lr = comparisonDelegate(left, right);

				var swapme =
					(lr <= 0 && comparisonDelegate(left, k) < 0)
						? ci
						: (lr > 0 && comparisonDelegate(right, k) < 0)
							  ? (ci + 1)
							  : -1;

				if (swapme == -1)
					return;

				Swap(i, swapme);
				i = swapme;
			}
		}

		private void Swap(int i, int j)
		{
			var tmp = array[i];
			array[i] = array[j];
			array[j] = tmp;

			if (!needNotifyIndexChange) return;

			notifyIndexChange(array[i], i);
			notifyIndexChange(array[j], j);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return array.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerable<T> GetItemsSameScoreAsMin()
		{
			var min = Min;

			var stack = new Stack<int>();
			stack.Push(0);

			while (stack.Count > 0)
			{
				var i = stack.Pop();

				var item = array[i];
				yield return item;

				var ci = 2 * i + 1;

				if (ci >= array.Count) continue;

				var left = array[ci];

				if (ci == array.Count - 1)
				{
					if (comparisonDelegate(left, min) == 0) stack.Push(ci);
					continue;
				}

				var right = array[ci + 1];

				if (comparisonDelegate(right, min) == 0) stack.Push(ci + 1);
			}
		}
	}
}