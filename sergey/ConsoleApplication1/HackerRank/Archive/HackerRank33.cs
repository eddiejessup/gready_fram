using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApplication1.Helpers.GameTheoryHelper;
using static ConsoleApplication1.Helpers.ArrayHelper;
using ConsoleApplication1.Helpers;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank33
	{
		public void Go()
		{
			Compare();
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				var n = rnd.Next(1, 5);
				var A = RandomGenerator.RandomLongArray(rnd, n, 1, 10);
				var B = RandomGenerator.RandomLongArray(rnd, n, 1, 10);
				var brute = Solve_Sums(A, B, n);
				var actual = Solve(A, B, n);

				if (actual != brute)
				{
					Console.WriteLine(A.Join());
					Console.WriteLine(B.Join());
					Console.WriteLine(new { n, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static long Solve_Sums(long[] A, long[] B, int n)
		{
			var sums = A.Zip(B, (x, y) => x + y).ToList();
			var a = A.ToList();
			var b = B.ToList();
			var diff = 0L;
			for (var m = 0; m < n; m++)
			{
				var maxI = sums.IndexOfElementWithMax(s => s);

				diff += (m % 2 == 0) ? a[maxI] : -b[maxI];

				sums.RemoveAt(maxI);
				a.RemoveAt(maxI);
				b.RemoveAt(maxI);
			}
			return diff;
		}

		public static long Solve(long[] A, long[] B, int n)
		{
			var ai = GetSortedIndices(n, (x, y) => A[x].CompareTo(A[y]));
			var bi = GetSortedIndices(n, (x, y) => B[x].CompareTo(B[y]));

			var busy = new bool[n];

			var diff = 0L;

			var a = n - 1;
			var b = n - 1;

			for (var m = 0; m < n; m++)
			{
				while (busy[ai[a]]) a--;
				while (busy[bi[b]]) b--;

				var isFirst = m % 2 == 0;

				var gain = isFirst ? A[ai[a]] : B[bi[b]];
				var damg = isFirst ? B[bi[b]] : A[ai[a]];

				var gainDamg = isFirst ? B[ai[a]] : A[bi[b]];
				var damgGain = isFirst ? A[bi[b]] : B[ai[a]];

				var doGain = gain > damg || (gain == damg && gainDamg >= damgGain);

				busy[doGain ^ isFirst ? bi[b] : ai[a]] = true;

				var ddiff = doGain ? gain : damgGain;

				diff += (isFirst ? 1 : -1) * ddiff;

				//     Console.WriteLine(new {isFirst, gain, damg, doGain, ddiff});
				//     Console.WriteLine(string.Join(" ", busy));
			}

			return diff;
		}

		public void ShowGr()
		{
			var M = 50;

			var gr = new int[M + 1];
			for (var i = 2; i <= M; i++)
			{
				var sub = new HashSet<int>();
				for (var j = 1; j < i; j++)
					if (i % j == 0)
						sub.Add(gr[j]);
				gr[i] = SpragueGrundy(sub);
			}

			for (var i = 0; i < gr.Length; i++)
				Console.WriteLine(new { i, gr = gr[i] });
		}
	}
}
