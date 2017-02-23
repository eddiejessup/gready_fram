using System;
using System.Collections.Generic;

namespace ConsoleApplication1.Helpers
{
	public class DelegateEqualityComparer<T> : IEqualityComparer<T>
	{
		private readonly Func<T, T, bool> comparer;

		public DelegateEqualityComparer(Func<T, T, bool> comparer)
		{
			this.comparer = comparer;
		}

		public bool Equals(T x, T y)
		{
			return comparer(x, y);
		}

		public int GetHashCode(T obj)
		{
			throw new NotSupportedException();
		}
	}

	public static class DelegateEqualityComparer
	{
		public static DelegateEqualityComparer<T> CreateByExample<T>(T example, Func<T, T, bool> comparer)
		{
			return new DelegateEqualityComparer<T>(comparer);
		}
	}
}