using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

// https://www.hackerrank.com/challenges/square-subsequences

namespace ConsoleApplication1.HackerRank
{
	class HackerRank20
	{
		public void Go()
		{
			//SolveBrute("AAA BBB");
			//Console.WriteLine(Solve("DABDBB"));
			Performance();
			//Compare();
		}

		public static void Performance()
		{
			// 8455
			// 8120

			var rnd = new Random(1337);
			for (var t = 1; t <= 20; t++)
			{
				var str = RandomGenerator.RandomString(rnd, "ABCDE", 200);
				Console.WriteLine(Solve(str));
			}
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				var len = rnd.Next(3, 8);
				var str = RandomGenerator.RandomString(rnd, "ABCDE", len);
				var brute = SolveBrute(str);
				var actual = Solve(str);

				if (actual != brute)
				{
					Console.WriteLine(new { str, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static ulong SolveBrute(string str)
		{
			var aarr = str.ToCharArray();

			var count = 0;

			for (var len = 2; len <= str.Length; len += 2)
			{
				foreach (var child in new Combinations<char>(aarr, len, GenerateOption.WithoutRepetition))
				{
					var s = string.Join("", child);
					if (IsSquare(s))
					{
					//	Console.WriteLine(s);
						count++;
					}
				}
			}

			return (ulong)count;
		}

		private static bool IsSquare(string str)
		{
			var half = str.Length / 2;
			for (var i = 0; i < half; i++)
				if (str[i] != str[half + i])
					return false;
			return true;
		}

		public const ulong MOD = 1000000007ul;

		public static ulong Solve(string str)
		{
			if (str.Length <= 1) return 0;
			if (str.Length == 2) return str[0] == str[1] ? 1ul : 0;


			var pairs = new List<int>[str.Length];
			var revPairs = new List<int>[str.Length];

			for (var i = 0; i < str.Length; i++)
			{
				pairs[i] = new List<int>();
				revPairs[i] = new List<int>();
			}
			for (var i = 0; i < str.Length; i++)
			{
				for (var j = i + 1; j < str.Length; j++)
					if (str[i] == str[j])
					{
						pairs[i].Add(j);
						revPairs[j].Add(i);
					}
			}
			for (var i = 0; i < str.Length; i++)
				pairs[i].Reverse();

			var sum = 0ul;
			for (var r = 1; r < str.Length; r++)
			{
				var F = new ulong[str.Length, str.Length];

				// i = r - 1:
				for (var j = str.Length - 1; j >= r; j--)
				{
					var sumF = 0ul;

					foreach (var ak in pairs[r - 1])
						if (ak >= j)
							sumF = (sumF + 1ul) % MOD;

					F[r - 1, j] = sumF;
				}

				for (var i = r - 2; i >= 0; i--)
				{
					var pairsI = pairs[i];

					for (var j = str.Length - 1; j >= r; j--)
					{
						var sumF = F[i + 1, j];

						foreach (var ak in pairsI)
							if (ak >= j)
								sumF = (sumF + 1ul + ((ak == str.Length - 1) ? 0ul : F[i + 1, ak + 1])) % MOD;
							else
								break;

						F[i, j] = sumF;
					}
				}

				{
					var sumF = 0ul;
					foreach (var i in revPairs[r])
						sumF = (sumF + 1ul + ((i == r - 1 || r == str.Length - 1) ? 0 : F[i + 1, r + 1])) % MOD;
					sum = (sum + sumF) % MOD;
				}
			}
			return sum;
		}

		public static ulong Solve_N5(string str)
		{
			if (str.Length <= 1) return 0;
			if (str.Length == 2) return str[0] == str[1] ? 1ul : 0; 

			var pairs = new List<int>[str.Length];
			for (var i = 0; i < str.Length; i++)
			{
				var list = new List<int>();
				for (var j = i + 1; j < str.Length; j++)
					if (str[i] == str[j])
						list.Add(j);
				list.Reverse();
				pairs[i] = list;
			}

			var K = new ulong[str.Length, str.Length, str.Length];

			for (var i = str.Length - 1; i >= 0; i--)
			{
				foreach (var j in pairs[i])
				{
					for (var k = j - 1; k >= i; k--)
					{
						var sum = 0ul;

						for (var ai = i + 1; ai <= k; ai++)
							foreach (var aj in pairs[ai])
								if (aj > j)
									sum = (sum + K[ai, aj, k]) % MOD;

						K[i, j, k] = 1 + sum;
					}
				}
			}

			{
				var sum = 0ul;
				for (var i = 0; i < str.Length; i++)
					foreach (var j in pairs[i])
						sum = (sum + K[i, j, j - 1]) % MOD;
				return sum;
			}
		}
	}
}
