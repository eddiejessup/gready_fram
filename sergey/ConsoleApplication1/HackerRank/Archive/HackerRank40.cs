using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApplication1.Helpers.MyMath;

// https://www.hackerrank.com/contests/projecteuler/challenges/euler148

namespace ConsoleApplication1.HackerRank
{
	class HackerRank40
	{
		const ulong mod = 1000000007ul;

		public void Go()
		{
			//Console.WriteLine(SolveBrute(100, 50));
			//Console.WriteLine(Solve(104, 12));
			Performance();
		}

		public static void Performance()
		{
			var fullDict = new Dictionary<ulong, ulong>();
			var S = 0ul;
			var N = (ulong)Math.Pow(10, 18);
			for (var t = 0ul; t < 3000; t++)
			{
				var r = Solve2(N, N - 100 * t, fullDict);
				S ^= r;
			}
			Console.WriteLine(S);
		}

		public static ulong Solve2(ulong N, ulong R, Dictionary<ulong, ulong> fullDict)
		{
			if (N == 0 || R == 0) return 0;

			var M = 1ul;
			while (7 * M < N) M = 7 * M;

			return Solve_Rec2(N, R, M, fullDict);
		}

		public static ulong Solve_Rec2(ulong N, ulong R, ulong M, Dictionary<ulong, ulong> fullDict)
		{
			var rows = N / M;
			var columns = R / M;

			var bCount = (columns <= rows) ? (rows - columns) : 0ul;
			var cCount = (columns <= rows) ? columns : (rows + 1);
			var dCount = ((N % M != 0) && (R % M != 0) && (columns <= rows)) ? 1ul : 0ul;
			var fullsCount = (columns <= rows) ? ((1 + columns) * columns / 2 + (rows - columns) * columns) : ((1 + rows) * rows / 2);

			if (M == 1)
				return fullsCount;

			var nextM = M / 7;

			var count = 0ul;

			if (fullsCount > 0)
			{
				ulong F1;
				if (!fullDict.TryGetValue(nextM, out F1))
				{
					F1 = Solve_Rec2(M, M, nextM, fullDict);
					fullDict[nextM] = F1;
				}

				count = (count + fullsCount * F1 % mod) % mod;
			}

			if (bCount > 0)
			{
				var B1 = Solve_Rec2(M, R % M, nextM, fullDict);

				count = (count + bCount * B1 % mod) % mod;
			}

			if (cCount > 0)
			{
				var C1 = Solve_Rec2(N % M, M, nextM, fullDict);

				count = (count + cCount * C1 % mod) % mod;
			}

			if (dCount > 0)
			{
				var D1 = Solve_Rec2(N % M, R % M, nextM, fullDict);

				count = (count + D1) % mod;
			}

			return count;
		}

		public static ulong Solve(ulong N, ulong R)
		{
			if (N == 0 || R == 0) return 0;

			var M = 1ul;
			while (7 * M < N) M = 7 * M;

			var count = Solve_Rec(N, R, M);
			return count;
		}

		public static ulong Solve_Rec(ulong N, ulong R, ulong M)
		{
			var fullsCount = 0ul;
			var bCount = 0ul;
			var cCount = 0ul;
			var dCount = 0ul; ;

			for (var i = 0ul; i < 7; i++)
				for (var j = 0ul; j <= i; j++)
				{
					var u = i * M + 1;
					var d = i * M + M;
					var l = j * M + 1;
					var r = j * M + M;

					if (N >= d && R >= r) fullsCount++;
					else if (N >= d && R >= l) bCount++;
					else if (R >= r && N >= u) cCount++;
					else if (R >= l && N >= u) dCount++;
				}

			if (M == 1)
				return fullsCount;

			var F1 = Solve_Rec(M, M, M / 7);

			var B1 = Solve_Rec(M, R % M, M / 7);

			var C1 = Solve_Rec(N % M, M, M / 7);

			var D1 = Solve_Rec(N % M, R % M, M / 7);

			var full = F1 * fullsCount % mod;
			var B = B1 * bCount % mod;
			var C = C1 * cCount % mod;
			var D = D1 * dCount;

			var count = (full + B + C + D) % mod;
			return count;
		}

		public static ulong Show(ulong N, ulong R)
		{
			if (N == 0 || R == 0) return 0;

			N--;
			R--;

			var factorials = new BigInteger[N + 1];
			factorials[0] = 1;
			for (var i = 1ul; i <= N; i++)
				factorials[i] = factorials[i - 1] * i;

			var count = 0ul;

			for (var n = 0ul; n <= N; n++)
			{
				for (var r = 0ul; r <= Math.Min(R, n); r++)
				{
					var cnk = factorials[n] / factorials[r] / factorials[n - r];
					cnk %= 7;
					Console.Write(cnk == 0 ? " " : cnk.ToString());
					if (cnk != 0)
						count = (count + 1) % mod;
				}
				Console.WriteLine();
			}

			return count;
		}

		public static void Compare()
		{
			var fullDict = new Dictionary<ulong, ulong>();
			var rnd = new Random(1337);
			for (var t = 0; t < 2000; t++)
			{
				var N = (ulong)rnd.Next(0, 100000);
				var R = (ulong)rnd.Next(0, (int)N + 1);
				var brute = Solve(N, R);
				var actual = Solve2(N, R, fullDict);

				if (actual != brute)
				{
					Console.WriteLine(new { t, N, R, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static ulong SolveBrute(ulong N, ulong R)
		{
			if (N == 0 || R == 0) return 0;

			N--;
			R--;

			var factorials = new BigInteger[N + 1];
			factorials[0] = 1;
			for (var i = 1ul; i <= N; i++)
				factorials[i] = factorials[i - 1] * i;

			var count = 0ul;
			for (var n = 0ul; n <= N; n++)
				for (var r = 0ul; r <= Math.Min(R, n); r++)
				{
					var cnk = factorials[n] / factorials[r] / factorials[n - r];
					if (cnk % 7 != 0)
						count = (count + 1) % mod;
				}

			return count;
		}
	}
}
