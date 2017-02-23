using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank60
	{
		public void Go()
		{
			//Console.WriteLine(SolveBrute("abb", 10));
			//CheckSolve();
			Compare();
			//Console.WriteLine(FindPeriodLength(RandomGenerator.RandomString(new Random(), "abc", 1000000)));
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				if (t % 1000 == 0) Console.WriteLine(t);
				var len = rnd.Next(1, 7);
				var str = RandomGenerator.RandomString(rnd, "ab", len);
				var m = rnd.Next(0, 15);
				var brute = SolveBrute(str, m);
				var actual = Solve(str, (ulong)m);

				if (actual != brute)
				{
					Console.WriteLine(new { t, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static void CheckSolve()
		{
			foreach (var s in new[]
			{
				"a",
				"ab",
				"abc",
				"aa",
				"aaaaaa",
				"ababab",
				"abcabcabc",
				"abab",
				"abbabbabb",
			})
			{
				Console.WriteLine(new { s, period = FindPeriodLength(s) });
			}
		}

		//private static ulong MOD = 1000000007ul;
		private static ulong MOD = 5ul;

		public static ulong Solve(string s, ulong m)
		{
			var period = FindPeriodLength(s);

			var result = period == 0
				? (m / (ulong)s.Length)
				: (m / period);

			return result % MOD;
		}

		public static ulong FindPeriodLength(string s)
		{
			var ss = s + s;

			var i = ss.IndexOf(s, 1);

			if (i >= s.Length)
				return 0ul;
			else
				return (ulong)i;
		}

		public static ulong SolveBrute(string s, int m)
		{
			var alphabet = s.Distinct().ToArray();
			var count = 0ul;
			for (var len = 1; len <= m; len++)
			{
				foreach (var comb in new Variations<char>(alphabet, len, GenerateOption.WithRepetition))
				{
					var t = string.Join("", comb);
					if (s + t == t + s)
					{
						count++;
					//	Console.WriteLine(t);
					}
				}
			}

			return count % MOD;
		}
	}
}
