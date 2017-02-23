using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	// https://www.hackerrank.com/challenges/grid-walking

	public class HackerRank31
	{
		public void Go()
		{
			Compare();
		}

		public static void CompareOneDimension()
		{
			var d = 100;
			var xUp = 15;
			var mUp = 20;

			var table = new ulong[100, 100];

			for (var x = 1; x <= xUp; x++)
				for (var m = 1; m <= mUp; m++)
					if (x < m - 1 && (x - m) % 2 == 0)
					{
						var brute = Solve_Brute(new[] { x }, new[] { d }, m);
						table[x, m] = (1ul << m) - brute;
					}

			for (var x = 1; x <= xUp; x++)
			{
				for (var m = 1; m <= mUp; m++)
					if (x < m - 1 && (x - m) % 2 == 0)
					{
						var guess = x > m ? 0 :
							x == m ? 1 :
							x == m - 1 ? 2 :
							x == m - 2 ? 4 + x :
							(x - m) % 2 == 0 ? 1 : 2;
						var diff = table[x, m];
						var a = diff - table[x, m - 2] * 4;

						//if (x < m - 2)
						//	if ((x % 2 == 0 && (a % ((ulong)x / 2)) != 0) || (x % 2 == 1 && (a % (ulong)x) != 0))
						//		throw new InvalidOperationException();

						var b = x % 2 == 0 ? (a / ((ulong)x / 2)) : (a / (ulong)x);


						Console.WriteLine(new { d, x, m, diff, guess, a });
					}
				Console.WriteLine();
			}
		}

		public static void CheckModInverse()
		{
			var mod = 101ul;
			//var mod = MOD;

			var N = 13;
			var factorials = new ulong[N + 1];
			factorials[0] = 1;
			for (var i = 1ul; i < (ulong)factorials.Length; i++)
				factorials[i] = factorials[i - 1] * i;

			var exp = new ulong[N + 1, N + 1];
			for (var i = 1; i <= N; i++)
				for (var j = 1; j <= i; j++)
					exp[i, j] = (factorials[i] / (factorials[i - j] * factorials[j])) % mod;

			var act = new ulong[N + 1, N + 1];
			for (var i = 1; i <= N; i++)
				for (var j = 1; j <= i; j++)
					act[i, j] = (((factorials[i] % mod)
									* MyMath.ModularInverse(factorials[i - j], mod)
										% mod)
											* MyMath.ModularInverse(factorials[j], mod))
												% mod;

			Console.WriteLine(exp.ToStringData());
			Console.WriteLine();
			Console.WriteLine(act.ToStringData());
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 100; t++)
			{
				var len = rnd.Next(1, 5);
				var D = RandomGenerator.RandomIntArray(rnd, len, 1, 10);
				var X = new int[len];
				for (var i = 0; i < X.Length; i++)
					X[i] = rnd.Next(1, D[i] + 1);
				var M = rnd.Next(1, 10);
				var brute = Solve_Brute(X, D, M);
				var actual = Solve(X, D, M);

				if (actual != brute)
				{
					Console.WriteLine(new { t, len, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		const ulong MOD = 1000000007ul;
		//const ulong MOD = 101ul;

		static ulong[] SolveOneDim(int x, int d, int M)
		{
			var dp = new ulong[d + 2, M + 1];

			for (var xi = 1; xi <= d; xi++)
			{
				dp[xi, 0] = 1;
				dp[xi, 1] = (xi > 1 && d > 1 ? 1ul : 0) + (xi < d ? 1ul : 0);
			}

			for (var m = 2; m <= M; m++)
				for (var xi = 1; xi <= d; xi++)
					dp[xi, m] = (dp[xi - 1, m - 1] + dp[xi + 1, m - 1]) % MOD;

			var result = new ulong[M + 1];
			for (var m = 0; m <= M; m++)
				result[m] = dp[x, m];
			return result;
		}

		static ulong Solve(int[] X, int[] D, int M)
		{
			var factorials = new ulong[301];
			factorials[0] = 1;
			for (var i = 1ul; i < (ulong)factorials.Length; i++)
				factorials[i] = factorials[i - 1] * i % MOD;

			var inverses = new ulong[301];
			for (var i = 0ul; i < (ulong)factorials.Length; i++)
				inverses[i] = MyMath.ModularInverse(factorials[i], MOD);

			var T = new ulong[X.Length, M + 1];
			for (var di = 0; di < X.Length; di++)
			{
				var t = SolveOneDim(X[di], D[di], M);
				for (var m = 0; m <= M; m++)
					T[di, m] = t[m];
			}

			var counts = new ulong[M + 1];
			for (var m = 0; m <= M; m++)
				counts[m] = T[0, m];

			for (var di = 1; di < X.Length; di++)
			{
				var newCounts = new ulong[M + 1];
				for (var m = 0; m <= M; m++)
				{
					var sum = 0ul;

					for (var i = 0; i <= m; i++)
					{
						var g = factorials[m] * inverses[m - i] % MOD * inverses[i] % MOD;
						var h = counts[m - i] * T[di, i] % MOD * g % MOD;
						sum = (sum + h) % MOD;
					}

					newCounts[m] = sum;
				}

				var tmp = newCounts;
				newCounts = counts;
				counts = tmp;
			}

			return counts[M];
		}

		static ulong Solve_Brute(int[] X, int[] D, int M)
		{
			if (M == 0) return 1ul;
			var sum = 0ul;
			for (var i = 0; i < X.Length; i++)
			{
				if (X[i] > 1 && D[i] > 1)
				{
					X[i]--;
					sum = (sum + Solve_Brute(X, D, M - 1)) % MOD;
					X[i]++;
				}
				if (X[i] < D[i])
				{
					X[i]++;
					sum = (sum + Solve_Brute(X, D, M - 1)) % MOD;
					X[i]--;
				}
			}
			return sum;
		}
	}
}
