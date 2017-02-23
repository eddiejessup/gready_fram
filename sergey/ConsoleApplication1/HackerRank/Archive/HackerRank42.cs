using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApplication1.Helpers.MyMath;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank42
	{
		private static ulong MOD = 1000000007ul;
		//private static ulong MOD = 11ul;

		public void Go()
		{
			//foreach (var arr in SolveBrute(new[] { 0L, 1, -1, 3, 2, -1, 2, -1 }, 1))
			//	Console.WriteLine(arr.Join());
			//Console.WriteLine(Solve(new[] { -1L, -1 }));
			//Console.WriteLine(Solve(new[] { -1L, -1, -1 }));
			//Console.WriteLine(Solve_N2(new[] { 0L, 1, -1, 3, 2, -1, 2, -1 }));
			//Console.WriteLine(Solve(new[] { 0L, -1, 0, 3, 4 }));
			//Console.WriteLine(SolveBrute(new[] { 0L, -1, 2, -1, 3, -1 }));
			Compare();
		}

		public static void Performance()
		{
			var rnd = new Random(1337);

			var len = 500000;

			var A = ArrayHelper.Create(len, i => -1L);

			var timer = Stopwatch.StartNew();
			Console.WriteLine(Solve(A));
			Console.WriteLine("Solve time: " + timer.ElapsedMilliseconds);
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			var notzero = 0;

			var Aproto = ArrayHelper.Create(50, i => -1L);

			for (var t = 0; t < 2000000; t++)
			{
				var len = rnd.Next(1, 5);

				var A = new long[len];
				for (var i = 1; i < A.Length; i++)
					A[i] = rnd.Next(-1, i + 1);

				var brute = SolveBrute(A.ToArray());
				var actual = Solve(A);

				if (actual != brute)
				{
					Console.WriteLine(A.Join());
					Console.WriteLine(new { t, brute, actual });
					throw new InvalidOperationException();
				}

				if (brute != 0)
					notzero++;
			}
			Console.WriteLine("Not zeroes " + notzero);
		}

		public static ulong Solve(long[] A)
		{
			if (A.Length == 1) return 1;

			var N = (ulong)A.Length;

			var rems = new long[A.Length + 1];
			for (var i = 0; i < rems.Length; i++)
				rems[i] = -1;

			for (var k = 2ul; k <= N; k++)
				for (var m = k; m <= N; m += k)
				{
					var am = A[m - 1];
					if (am >= 0)
					{
						var oldRem = rems[k];
						var newRem = am % (long)k;
						if (oldRem >= 0 && oldRem != newRem)
							return 0;
						rems[k] = newRem;
					}
				}

			var count = 1ul;

			var divisors = new List<ulong>[A.Length + 1];
			for (var i = 0; i < divisors.Length; i++)
				divisors[i] = new List<ulong>();

			for (var k = 2ul; k <= N; k++)
				for (var m = k; m <= N; m += k)
					if (A[m - 1] < 0)
						divisors[m].Add(k);

			for (var m = 2ul; m <= N; m++)
			{
				if (A[m - 1] < 0)
				{
					var x = 1ul;
					foreach (var k in divisors[m])
					{
						if (rems[k] >= 0)
							x = x * k / ExtendedEuclidGcd(x, k);
						else
							rems[k] = 1;
					}
					count = count * (m / x) % MOD;
				}
			}

			return count;
		}

		public static ulong Solve_N2(long[] A)
		{
			if (A.Length == 1) return 1;

			var N = (ulong)A.Length;

			var rems = new long[A.Length + 1];
			for (var i = 0; i < rems.Length; i++)
				rems[i] = -1;

			for (var k = 2ul; k <= N; k++)
				for (var m = k; m <= N; m += k)
				{
					var am = A[m - 1];
					if (am >= 0)
					{
						var oldRem = rems[k];
						var newRem = am % (long)k;
						if (oldRem >= 0 && oldRem != newRem)
							return 0;
						rems[k] = newRem;
					}
				}

			var count = 1ul;

			for (var m = 2ul; m <= N; m++)
			{
				var am = A[m - 1];
				if (am < 0)
				{
					var x = 1ul;
					for (var k = 2ul; k <= m; k++)
						if (m % k == 0)
						{
							if (rems[k] >= 0)
								x = LCM(x, k);
							else
								rems[k] = 1;
						}
					count = count * (m / x) % MOD;
				}
			}

			return count;
		}

		public static ulong SolveBrute(long[] A)
		{
			if (!CheckStart(A)) return 0;
			var count = 0ul;
			foreach (var arr in SolveBrute(A, 1))
				count++;
			return count % MOD;
		}

		private static bool CheckStart(long[] A)
		{
			for (var m = 2; m <= A.Length; m++)
			{
				var am = A[m - 1];
				if (am < 0) continue;

				for (var k = 2; k <= m / 2; k++)
				{
					var ak = A[k - 1];
					if (ak >= 0 && (m % k == 0) && ((am % k) != ak))
						return false;
				}
			}
			return true;
		}

		public static IEnumerable<long[]> SolveBrute(long[] A, int m)
		{
			if (A.Length == 1 || m == A.Length + 1)
				yield return A;

			else
			{
				var am = A[m - 1];

				if (am >= 0)
				{
					if (Check(A, am, m))
						foreach (var arr in SolveBrute(A, m + 1))
							yield return arr;
				}

				else
				{
					for (var ai = 0; ai <= m - 1; ai++)
					{
						if (!Check(A, ai, m)) continue;

						A[m - 1] = ai;
						foreach (var arr in SolveBrute(A, m + 1))
							yield return arr;
					}

					A[m - 1] = -1;
				}
			}
		}

		private static bool Check(long[] A, long am, int m)
		{
			for (var k = 2; k <= m / 2; k++)
			{
				var ak = A[k - 1];
				if ((m % k == 0) && ((am % k) != ak))
					return false;
			}
				
			return true;
		}
	}
}
