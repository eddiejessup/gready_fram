using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank41
	{
		public void Go()
		{
			Performance();
		}

		public static void Performance()
		{
			var rnd = new Random(1337);
			var len = 1000000;
			var X = RandomGenerator.RandomLongArray(rnd, len, -100000, 100000);
			var Y = RandomGenerator.RandomLongArray(rnd, len - 1, -100000, 100000);
			var addme = X.Sum() - Y.Sum();
			Y = Y.ConcatElementToArray(addme);

			var timer = Stopwatch.StartNew();
			Console.WriteLine(Solve(X, Y));
			Console.WriteLine("Solve time: " + timer.ElapsedMilliseconds);
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 10000; t++)
			{
				var len = rnd.Next(2, 6);
				var X = RandomGenerator.RandomLongArray(rnd, len, -5, 5);
				var Y = RandomGenerator.RandomLongArray(rnd, len - 1, -5, 5);
				var addme = X.Sum() - Y.Sum();
				Y = Y.ConcatElementToArray(addme);
				var brute = SolveBrute(X, Y);
				var actual = Solve(X, Y);

				if (actual != brute)
				{
					Console.WriteLine("X=" + X.Join());
					Console.WriteLine("Y=" + Y.Join());
					Console.WriteLine(new { t, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static long SolveBrute(long[] X, long[] Y)
		{
			var best = long.MaxValue;

			foreach (var cand in new Permutations<int>(ArrayHelper.Create(X.Length, i => i), GenerateOption.WithoutRepetition))
			{
				var Z = cand.Select(i => Y[i]).ToArray();

				var count = 0L;

				for (var i = 0; i < X.Length; i++)
					if (X[i] != Z[i])
					{
						var op = Z[i] - X[i];

						count += Math.Abs(op);

						for (var j = i + 1; j < X.Length; j++)
						{
							if (op > 0 && Z[j] < X[j])
							{
								op--;
								Z[j]++;
								j--;
							}
							else if (op < 0 && Z[j] > X[j])
							{
								op++;
								Z[j]--;
								j--;
							}

							if (op == 0)
								break;
						}
					}

				best = Math.Min(count, best);
			}

			return best;
		}

		public static long Solve(long[] X, long[] Y)
		{
			Array.Sort(X, 0, X.Length);
			Array.Sort(Y, 0, Y.Length);

			var count = 0L;
			var debt = 0L;

			for (var i = 0; i < X.Length; i++)
			{
				var op = Y[i] - X[i];

				if (op > 0 && debt < 0)
				{
					var d = Math.Min(op, -debt);
					debt += d;
					op -= d;
				}
				else if (op < 0 && debt > 0)
				{
					var d = Math.Min(-op, debt);
					debt -= d;
					op += d;
				}

				debt += op;
				count += Math.Abs(op);
			}

			return count;
		}
	}
}
