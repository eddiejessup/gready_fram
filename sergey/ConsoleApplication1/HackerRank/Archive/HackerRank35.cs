using ConsoleApplication1.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank35
	{
		public void Go()
		{
			Compare();
		}

		public static void Compare()
		{
			var dp = CalcCache(2000);

			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				var len = rnd.Next(3, 10);
				var A = RandomGenerator.RandomIntArray(rnd, len, 0, 2);
				var brute = SolveBrute(A);
				var actual = Solve(A, dp);

				if (actual != brute)
				{
					Console.WriteLine(A.Join());
					Console.WriteLine(new { t, len, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static void ShowFewGames()
		{
			var len = 5;

			for (var d0 = 0; d0 <= len; d0++)
				for (var d1 = 0; d1 <= len; d1++)
					for (var d2 = 0; d2 <= len; d2++)
					{
						var D = new[] { d0, d1, d2 };
						var isBalsaWin = SolveBrute(D, 0, true);
						Console.WriteLine($"D=[ {D.Join()} ] " + (isBalsaWin ? "Balsa" : "Koca"));
					}
		}

		struct Index : IEquatable<Index>
		{
			public bool IsBalsa;
			public int B;
			public int D1;
			public int D2;

			public Index(bool isBalsa, int b, int d1, int d2) : this()
			{
				IsBalsa = isBalsa;
				B = b;
				D1 = d1;
				D2 = d2;
			}

			public bool Equals(Index other)
			{
				return IsBalsa = other.IsBalsa && B == other.B && D1 == other.D1 && D2 == other.D2;
			}
		}

		// (w, b, d1, d2) Кто победит в текущей ситуации b, (d1, d2) если ход w=1 (Balsa), w=0 (Koca). True означает победу Balsa

		public static bool[,,,] CalcCache(int maxD)
		{
			var dp = new bool[2, 3, maxD + 1, maxD + 1];

			for (var d1 = 0; d1 <= maxD; d1++)
				for (var d2 = 0; d2 <= maxD; d2++)
					for (var w = 0; w <= 1; w++)
						for (var b = 0; b <= 2; b++)
						{
							if (d1 == 0 && d2 == 0)
								dp[w, b, 0, 0] = w == 1 ? b > 0 : b == 0;

							else
							{
								var nextBd1 = (w == 1 ? (b + 1) : (b + 2)) % 3;
								var nextBd2 = (w == 1 ? (b + 2) : (b + 1)) % 3;

								if (d1 == 0)
									dp[w, b, 0, d2] = !dp[1 - w, nextBd2, 0, d2 - 1];

								else if (d2 == 0)
									dp[w, b, d1, 0] = !dp[1 - w, nextBd1, d1 - 1, 0];

								else
									dp[w, b, d1, d2] = (!dp[1 - w, nextBd1, d1 - 1, d2]) || (!dp[1 - w, nextBd2, d1, d2 - 1]);
							}
						}

			return dp;
		}

		public static bool Solve(int[] A, bool[,,,] dp)
		{
			var D = new int[3];
			foreach (var a in A)
				D[a % 3]++;

			return !dp[1, 0, D[1], D[2]];
		}

		public static bool SolveBrute_2(int[] A)
		{
			var D = new int[3];
			foreach (var a in A)
				D[a % 3]++;

			D[0] = 0;

			return !SolveBrute_2(D, 0, true);
		}

		public static bool SolveBrute_2(int[] D, int b, bool isBalsa)
		{
			var sum = D.Sum();
			if (sum == 0) return isBalsa ? b > 0 : b == 0;

			for (var i = 0; i < D.Length; i++)
				if (D[i] > 0)
				{
					var prev = D[i];
					D[i] = prev - 1;
					var win = SolveBrute(D, (isBalsa ? (b + i) : (b + 3 - i)) % 3, !isBalsa);
					D[i] = prev;

					if (!win) return true;
				}

			return false;
		}

		public static bool SolveBrute(int[] A)
		{
			var D = new int[3];
			foreach (var a in A)
				D[a % 3]++;

			var result = !SolveBrute(D, 0, true);

			return result;
		}

		public static bool SolveBrute(int[] D, int b, bool isBalsa)
		{
			var sum = D.Sum();
			if (sum == 0) return isBalsa ? b > 0 : b == 0;

			for (var i = 0; i < D.Length; i++)
				if (D[i] > 0)
				{
					var prev = D[i];
					D[i] = prev - 1;
					var win = SolveBrute(D, (isBalsa ? (b + i) : (b + 3 - i)) % 3, !isBalsa);
					D[i] = prev;

					if (!win) return true;
				}

			return false;
		}
	}
}
