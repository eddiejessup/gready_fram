using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApplication1.DataTypes
{
	public struct Pair<T1, T2> : IEquatable<Pair<T1, T2>>
	{
		public Pair(T1 item1, T2 item2)
		{
			Item1 = item1;
			Item2 = item2;
		}

		private static readonly EqualityComparer<T1> item1Comparer = EqualityComparer<T1>.Default;
		private static readonly EqualityComparer<T2> item2Comparer = EqualityComparer<T2>.Default;

		public T1 Item1 { get; }
		public T2 Item2 { get; }

		public static readonly PairsComparer<T1, T2> Comparer = new PairsComparer<T1, T2>();

		public override int GetHashCode()
		{
			var h1 = item1Comparer.GetHashCode(Item1);
			var h2 = item2Comparer.GetHashCode(Item2);

			return ((h1 << 5) + h1) ^ h2;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(Pair<T1, T2> other)
		{
			return item1Comparer.Equals(Item1, other.Item1) && item2Comparer.Equals(Item2, other.Item2);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("(");
			sb.Append(Item1);
			sb.Append(", ");
			sb.Append(Item2);
			sb.Append(")");
			return sb.ToString();
		}
	}

	public class PairsComparer<T1, T2> : IEqualityComparer<Pair<T1, T2>>
	{
		private static readonly EqualityComparer<T1> item1Comparer = EqualityComparer<T1>.Default;
		private static readonly EqualityComparer<T2> item2Comparer = EqualityComparer<T2>.Default;

		public bool Equals(Pair<T1, T2> x, Pair<T1, T2> y)
		{
			return item1Comparer.Equals(x.Item1, y.Item1) && item2Comparer.Equals(x.Item2, y.Item2);
		}

		public int GetHashCode(Pair<T1, T2> obj)
		{
			return obj.GetHashCode();
		}
	}

	public static class PairCreate
	{
		public static Pair<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
		{
			return new Pair<T1, T2>(item1, item2);
		} 
	}
}