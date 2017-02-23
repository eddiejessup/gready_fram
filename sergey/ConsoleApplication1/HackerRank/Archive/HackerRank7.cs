using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank7
	{
		public void Go()
		{
			//Solve(new[] { -1, 2, -3 }, new[] { 1, -2, 3 });

			//Solve(new[] { -1, 0, 0, 0, 1, 1, 2, 3, 4 }, new[] { 2, -1, -3, -2, 1, 0, 5, 2, -10 });

			//Test_Solution();

			Test_Heavy();

			//var data = new Data(new[] { -1, 0, 0, 0, 1, 1, 2, 3, 4 });

			//	Console.WriteLine(new { split = GetSplitPoint(new[] { -3, -2, -1, 0, 1, 2, 3, 4, 5 }, 2) });

			//Test_GetLeftPartIndex();
			//Test_GetRightPartIndex();
		}

		public static void Test_Heavy()
		{
			var random = new Random(1337);

			var arr = Enumerable.Repeat(0, 500000).Select(_ => (long)(random.Next(4000) - 2000)).OrderBy(_ => _).ToArray();
			var q = Enumerable.Repeat(0, 500000).Select(_ => (long)(random.Next(4000) - 2000)).OrderBy(_ => _).ToArray();

			var timer = Stopwatch.StartNew();

			var result = Solve_Internal(arr, q).ToArray();

			Console.WriteLine("Solving time: " + timer.ElapsedMilliseconds);
		}

		public static void Test_Solution()
		{
			var random = new Random(1337);

			for (var i = 0; i < 100000; i++)
			{
				var len = random.Next(1, 10);
				var arr = Enumerable.Repeat(0, len).Select(_ => (long)(random.Next(10) - 5)).OrderBy(_ => _).ToArray();
				
				var q = Enumerable.Repeat(0, 10).Select(_ => (long)(random.Next(10) - 5)).OrderBy(_ => _).ToArray();
				var actualArray = Solve_Internal(arr, q).ToArray();

				var copyArr = arr.ToArray();
				for (var j = 0; j < q.Length; j++)
				{
					var x = q[j];

					for (var k = 0; k < copyArr.Length; k++)
						copyArr[k] += x;
					var expectedBelow = -copyArr.Where(k => k < 0).Sum();
					var expectedAbove = copyArr.Where(k => k > 0).Sum();

					var actual = actualArray[j];
					if (expectedBelow + expectedAbove != actual)
					{
						Console.WriteLine(arr.Join(","));
						Console.WriteLine(q.Join());
						Console.WriteLine(new { j, x, expectedBelow, expectedAbove, actual });
						throw new InvalidOperationException();
					}
				}
			}
		}

		public static void Test_GetLeftPartIndex()
		{
			var random = new Random();

			for (var i = 0; i < 10000; i++)
			{
				var len = random.Next(1, 8);
				var arr = Enumerable.Repeat(0, len).Select(_ => (long)(random.Next(10) - 5)).OrderBy(_ => _).ToArray();

				var x = (long)(random.Next(10) - 5);
				var actual = GetLeftPartIndex(arr, x);

				var isOk = true;

				if (arr[actual] == x)
				{
					isOk = actual == 0 || arr[actual - 1] < x;
				}
				else if (arr[actual] < x)
				{
					isOk = actual == arr.Length - 1 || arr[actual + 1] > x;
				}
				else // arr[actual] > x
				{
					isOk = actual == 0;
				}

				if (!isOk)
				{
					Console.WriteLine(arr.Join());
					Console.WriteLine(new { x, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static void Test_GetRightPartIndex()
		{
			var random = new Random();

			for (var i = 0; i < 10000; i++)
			{
				var len = random.Next(1, 8);
				var arr = Enumerable.Repeat(0, len).Select(_ => (long)(random.Next(10) - 5)).OrderBy(_ => _).ToArray();

				var x = (long)(random.Next(10) - 5);
				var actual = GetRightPartIndex(arr, x);

				var isOk = true;

				if (arr[actual] == x)
				{
					isOk = actual == arr.Length - 1 || arr[actual + 1] > x;
				}
				else if (arr[actual] > x)
				{
					isOk = actual == 0 || arr[actual - 1] < x;
				}
				else // arr[actual] > x
				{
					isOk = actual == arr.Length - 1;
				}

				if (!isOk)
				{
					Console.WriteLine(arr.Join());
					Console.WriteLine(new { x, actual });
					throw new InvalidOperationException();
				}
			}
		}

		// If array contains x: return most right index of x
		// Else: return most left index of item, that greater than x
		public static int GetRightPartIndex(IList<long> arr, long x)
		{
			var li = 0;
			var ri = arr.Count - 1;

			while (true)
			{
				var mi = (li + ri) / 2;

				if (mi == li)
					return arr[li] < x ? ri : arr[ri] == x ? ri : li;

				if (arr[mi] < x)
				{
					li = mi;
				}
				else if (arr[mi] == x)
				{
					li = mi;
				}
				else
				{
					ri = mi;
				}
			}
		}

		// If array contains x: return most left index of x
		// Else: return most right index of item, that less than x
		public static int GetLeftPartIndex(IList<long> arr, long x)
		{
			var li = 0;
			var ri = arr.Count - 1;

			while (true)
			{
				var mi = (li + ri) / 2;

				if (mi == li)
					return arr[ri] > x ? li : arr[li] == x ? li : ri;

				if (arr[mi] < x)
				{
					li = mi;
				}
				else if (arr[mi] == x)
				{
					ri = mi;
				}
				else
				{
					ri = mi;
				}
			}
		}

		public class Data
		{
			public List<long> Arr;
			public List<long> Counts;

			public long Min;
			public long Sum;
			public long Width;

			public long[] SumLeft;
			public long[] SumRight;
			public long[] WidthLeft;
			public long[] WidthRight;

			public Dictionary<long, long> cache = new Dictionary<long, long>();

			public Data(long[] arr)
			{
				var sorted = arr.OrderBy(i => i).ToArray();
				Arr = new List<long> { sorted[0] };
				Counts = new List<long> { 1 };
				for (var i = 1L; i < sorted.Length; i++)
				{
					if (sorted[i] == sorted[i - 1])
					{
						Counts[Counts.Count - 1]++;
					}
					else
					{
						Arr.Add(sorted[i]);
						Counts.Add(1);
					}
				}

				Min = Arr[0];
				for (var i = 0; i < Arr.Count; i++)
					Arr[i] -= Min;

				Sum = Enumerable.Range(0, Arr.Count).Sum(i => Arr[i] * Counts[i]);

				Width = sorted.Length;

				SumLeft = new long[Arr.Count];
				var a = 0L;
				for (var i = 0; i < Arr.Count; i++)
				{
					SumLeft[i] = a;
					a += Arr[i] * Counts[i];
				}

				SumRight = new long[Arr.Count];
				var b = 0L;
				var w = 0L;
				for (var i = Arr.Count - 1; i >= 0; i--)
				{
					SumRight[i] = b - w * Arr[i];
					b += Arr[i] * Counts[i];
					w += Counts[i];
				}

				WidthLeft = new long[Arr.Count];
				w = 0L;
				for (var i = 0; i < Arr.Count; i++)
				{
					WidthLeft[i] = w;
					w += Counts[i];
				}

				WidthRight = new long[Arr.Count];
				w = 0L;
				for (var i = Arr.Count - 1; i >= 0; i--)
				{
					WidthRight[i] = w;
					w += Counts[i];
				}
			}

			private long GetSumLeft(int index, bool include)
			{
				if (include)
				{
					if (index == Arr.Count - 1) return Sum;
					index++;
				}
				var result = SumLeft[index];
				return result;
			}

			private long GetReverseSumBelowZero(long zero, int index)
			{
				var include = Arr[index] < zero;

				var sum = GetSumLeft(index, include);

				var area = (WidthLeft[index] + (include ? Counts[index] : 0L)) * zero;

				return area - sum;
			}

			private long GetSumAboveZero(long zero, int index)
			{
				if (index == 0)
				{
					var areaRectangle = Min * Width;
					return Sum + areaRectangle;
				}

				var include = Arr[index] > zero;

				if (include)
					index--;

				var sum = SumRight[index];

				if (include)
					sum -= WidthRight[index] * (Arr[index + 1] - Arr[index] - (Arr[index + 1] - zero));

				return sum;
			}

			public long GetSums()
			{
				if (Arr.Count == 1) return Math.Abs(Min) * Counts[0];
				if (cache.ContainsKey(Min)) return cache[Min];

				var zero = -Min;

				var index = GetLeftPartIndex(Arr, zero);

				var below = GetReverseSumBelowZero(zero, index);

				var above =
					(index == Arr.Count - 1 && Arr[index] <= zero) ? 0
					: Arr[index] >= zero ? GetSumAboveZero(zero, index)
					: GetSumAboveZero(zero, index + 1);

				var result = below + above;

				cache[Min] = result;

				return result;
			}

			public void Add(long x)
			{
				Min += x;
			}
		}

		public static void Solve(long[] arr, long[] q)
		{
			var output = string.Join(Environment.NewLine, Solve_Internal(arr, q));
			Console.WriteLine(output);
		}

		public static IEnumerable<long> Solve_Internal(long[] arr, long[] q)
		{
			var data = new Data(arr);
			foreach (var x in q)
			{
				data.Add(x);
				yield return data.GetSums();
			}
		}
	}
}
