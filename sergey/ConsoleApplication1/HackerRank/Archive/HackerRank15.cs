using ConsoleApplication1.Helpers;
using System;
using System.Linq;

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank15
	{
		public void Go()
		{
			Solve("ATCAGCGTATACGCTG");
			Compare();
			//Console.WriteLine(Solve("GAAATAAA"));
		}

		public static void Compare()
		{
			var letters = "ACTG".ToCharArray();
			var rnd = new Random(1337);
			for (var t = 0; t < 1000; t++)
			{
				var len = rnd.Next(1, 10) * 4;
				var str = new string(Enumerable.Repeat(0, len).Select(i => letters.Random(rnd)).ToArray());

				var brute = SolveBruteForce(str);
				var actual = Solve(str);

				if (brute != actual)
				{
					Console.WriteLine(str);
					Console.WriteLine(new { brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static int SolveBruteForce(string s)
		{
			if ("ACTG".Select(c => s.Count(cc => cc == c)).AllSame())
				return 0;

			var opt = s.Length / 4;

			var best = int.MaxValue;
			for (var i = 0; i < s.Length; i++)
			{
				var left = s.Substring(0, i);

				var leftCounts = "ACTG".Select(c => left.Count(cc => cc == c)).ToArray();

				for (var len = 1; len + i <= s.Length; len++)
				{
					if (len >= best) break;

					var right = s.Substring(len + i);

					var rightCounts = "ACTG".Select(c => right.Count(cc => cc == c)).ToArray();

					var totalCounts = ArrayHelper.Merge(leftCounts, rightCounts, (a, b) => a + b);

					if (totalCounts.Any(c => c > opt))
						continue;

					var needChange = totalCounts.Sum(c => opt - c);
					if (needChange <= len)
						best = Math.Min(best, len);
				}
			}
			return best;
		}

		public static int Solve(string s)
		{
			var N = s.Length;

			var opt = N / 4;

			var lefts = new int[N + 1][];
			lefts[0] = new int[4];
			for (var i = 1; i <= N; i++)
			{
				var c = s[i - 1];
				var counts = new int[4];
				for (var d = 0; d < 4; d++)
					counts[d] = lefts[i - 1][d];
				counts[c == 'A' ? 0 : c == 'C' ? 1 : c == 'T' ? 2 : 3]++;
				lefts[i] = counts;
			}

			if (lefts[N].Sum(c => Math.Abs(c - opt)) == 0)
				return 0;

			var rights = new int[N + 1][];
			rights[N] = new int[4];
			for (var j = N - 1; j >= 0; j--)
			{
				var c = s[j];
				var counts = new int[4];
				for (var d = 0; d < 4; d++)
					counts[d] = j == N - 1 ? 0 : rights[j + 1][d];
				counts[c == 'A' ? 0 : c == 'C' ? 1 : c == 'T' ? 2 : 3]++;
				rights[j] = counts;
			}

			var best = int.MaxValue;
			for (var j = N; j >= 0; j--)
			{
				var right = rights[j];

				var bestI = BinarySearchHelper.BinarySearchLeftBiased(lefts, counts => -Distance(right, counts, opt));

				var distanceHere = Distance(right, lefts[bestI], opt);

				if (distanceHere < 0 || bestI >= j)
					continue;

				if (bestI >= 0)
					best = Math.Min(best, j - bestI);
			}

			return best;
		}

		private static int Distance(int[] a, int[] b, int opt)
		{
			var d = 0;
			for (var i = 0; i < a.Length; i++)
			{
				var s = a[i] + b[i];
				if (s > opt) return -1;
				d += s;
			}
			return d;
		}
	}
}
