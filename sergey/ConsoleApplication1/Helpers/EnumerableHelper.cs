using ConsoleApplication1.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1.Helpers
{
	public static class EnumerableHelper
	{
		public static IEnumerable<List<T>> ByBatches<T>(this IEnumerable<T> seq, int batchLength)
		{
			var list = new List<T>(batchLength);
			foreach (var item in seq)
			{
				if (list.Count == batchLength) list.Clear();

				list.Add(item);

				if (list.Count == batchLength)
					yield return list;
			}
		}

		public static IEnumerable<int> IndicesWhere<T>(this IEnumerable<T> seq, Func<T, bool> clause)
		{
			var i = 0;
			foreach (var item in seq)
			{
				if (clause(item))
					yield return i;
				i++;
			}
		}

		public static T ElementWithMax<T, TScore>(this IEnumerable<T> seq, Func<T, TScore> getScore) where TScore : IComparable
		{
			var best = default(T);
			var bestScore = default(TScore);
			var isFirst = true;

			foreach (var item in seq)
			{
				var score = getScore(item);

				if (isFirst)
				{
					best = item;
					bestScore = score;
					isFirst = false;
				}
				else if (score.CompareTo(bestScore) > 0)
				{
					best = item;
					bestScore = score;
				}
			}

			return best;
		}

		public static int IndexOfElementWithMax<T, TScore>(this IList<T> seq, Func<T, TScore> getScore) where TScore : IComparable
		{
			var bestScore = default(TScore);
			var bestIndex = -1;
			var isFirst = true;

			var i = 0;
			foreach (var item in seq)
			{
				var score = getScore(item);

				if (isFirst)
				{
					bestIndex = i;
					bestScore = score;
					isFirst = false;
				}
				else if (score.CompareTo(bestScore) > 0)
				{
					bestIndex = i;
					bestScore = score;
				}

				i++;
			}

			if (bestIndex < 0)
				throw new InvalidOperationException("Sequence is empty");

			return bestIndex;
		}

		public static int IndexOfElementWithMax(this IEnumerable<ulong> seq)
		{
			var i = 0;
			var max = ulong.MinValue;
			var maxI = -1;
			foreach (var item in seq)
			{
				if (maxI < 0 || item > max)
				{
					max = item;
					maxI = i;
				}
				i++;
			}
			return maxI;
		}

		public static KeyValuePair<T, int> MaxAndCount<T>(this IEnumerable<T> seq, Func<T, int> getScore)
		{
			var best = default(T);
			var score = Int32.MinValue;
			var count = 0;

			foreach (var item in seq)
			{
				var newScore = getScore(item);
				if (newScore > score)
				{
					score = newScore;
					best = item;
					count = 1;
				}
				else if (newScore == score)
					count++;
			}

			return new KeyValuePair<T, int>(best, count);
		}

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> seq)
		{
			return new HashSet<T>(seq);
		}

		public static void AddRange<T>(this HashSet<T> hashset, IEnumerable<T> seq)
		{
			foreach (var item in seq)
				hashset.Add(item);
		}

		public static List<List<T>> GroupByEquality<T>(this IEnumerable<T> source, Func<T, T, bool> equals)
		{
			var groups = new List<List<T>>();
			foreach (var item in source)
			{
				var item1 = item;
				var groupIndex = groups.FirstIndexOf(o => equals(item1, o[0]));

				if (groupIndex < 0)
					groups.Add(new List<T> { item });
				else
					groups[groupIndex].Add(item);
			}
			return groups;
		}

		public static bool AllByEquality<T>(this IEnumerable<T> source, Func<T, T, bool> equals)
		{
			var hasAny = false;
			var comparer = default(T);

			foreach (var item in source)
			{
				if (!hasAny)
				{
					comparer = item;
					hasAny = true;
				}
				else if (!@equals(comparer, item))
					return false;
			}

			return true;
		}

		public static List<T> DistinctByEquality<T>(this IEnumerable<T> source, Func<T, T, bool> equals)
		{
			var distincts = new List<T>();

			foreach (var item in source)
				if (distincts.All(d => !equals(d, item)))
					distincts.Add(item);

			return distincts;
		}

		public static void RemoveDuplicates<T>(this List<T> list, Func<T, T, bool> equals)
		{
			var toDelete = new HashSet<T>();

			for (var i = 0; i < list.Count; i++)
				for (var j = i + 1; j < list.Count; j++)
					if (equals(list[i], list[j]))
						toDelete.Add(list[j]);

			list.RemoveAll(toDelete.Contains);
		}

		public static IEnumerable<T> ConcatWithElement<T>(this IEnumerable<T> seq, T element)
		{
			foreach (var item in seq)
				yield return item;
			yield return element;
		}

		public static bool AllSame<T>(this IEnumerable<T> seq) where T : IEquatable<T>
		{
			var elem = default(T);
			var hasAny = false;

			foreach (var item in seq)
			{
				if (!hasAny)
				{
					hasAny = true;
					elem = item;
				}
				else
				{
					var areEqual = elem.Equals(item);
					if (!areEqual)
						return false;
				}
			}

			if (!hasAny)
				throw new InvalidOperationException("Sequence is empty");

			return true;
		}

		public static Pair<int, int> MinAndMax<T>(this IEnumerable<T> seq, Func<T, int> getValue)
		{
			var min = int.MaxValue;
			var max = int.MinValue;

			var hasAny = false;

			foreach (var item in seq)
			{
				hasAny = true;
				var val = getValue(item);
				if (min > val) min = val;
				if (max < val) max = val;
			}

			if (!hasAny)
				throw new InvalidOperationException("Sequence is empty");

			return new Pair<int, int>(min, max);
		}

		public static IEnumerable<T> GetEnumerableSafe<T>(this IEnumerable<T> seq)
		{
			return seq ?? Enumerable.Empty<T>();
		}

		public static bool AllTrues(this IEnumerable<bool> seq)
		{
			foreach (var item in seq)
				if (!item)
					return false;
			return true;
		}

		public static ulong Mul(this IEnumerable<int> ints)
		{
			var result = 1ul;
			foreach (var i in ints)
				result *= (ulong)i;
			return result;
		}

		public static ulong Mul(this IEnumerable<ulong> ulongs)
		{
			var result = 1ul;
			foreach (var i in ulongs)
				result *= i;
			return result;
		}

		public static ulong Sum(this IEnumerable<ulong> ulongs)
		{
			var result = 0ul;
			foreach (var i in ulongs)
				result += i;
			return result;
		}
	}
}