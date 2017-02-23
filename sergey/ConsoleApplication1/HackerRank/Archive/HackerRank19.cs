using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

// https://www.hackerrank.com/challenges/common-child

namespace ConsoleApplication1.HackerRank
{
	class HackerRank19
	{
		public void Go()
		{
			//Solve("BE", "E");

			Compare();

			//Console.WriteLine(Solve("ABAB", "BAB"));
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				var len1 = rnd.Next(1, 6);
				var len2 = rnd.Next(1, 6);
				var a = RandomGenerator.RandomString(rnd, "ABCDE", len1);
				var b = RandomGenerator.RandomString(rnd, "ABCDE", len2);
				var brute = SolveBrute(a, b);
				var actual = Solve(a, b);

				if (actual != brute.Length)
				{
					Console.WriteLine(new { a, b, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static string SolveBrute(string a, string b)
		{
			if (a == b) return a;

			var aarr = a.ToCharArray();

			var result = "";

			for (var len = 1; len <= a.Length; len++)
			{
				foreach (var child in new Combinations<char>(aarr, len, GenerateOption.WithoutRepetition))
				{
					if (IsChild(child, b))
					{
						result = new string(child.ToArray());
						break;
					}
				}
			}

			return result;
		}

		private static bool IsChild(IList<char> child, string s)
		{
			var j = -1;
			for (var i = 0; i < child.Count; i++)
			{
				for (j++; j < s.Length; j++)
					if (s[j] == child[i])
						break;

				if (j == s.Length)
					return false;
			}
			return true;
		}

		public static int Solve(string a, string b)
		{
			if (a.Length == 0 || b.Length == 0) return 0;

			var dp = new int[a.Length, b.Length];

			{
				dp[0, 0] = a[0] == b[0] ? 1 : 0;

				for (var j = 1; j < b.Length; j++)
					dp[0, j] = Math.Min(1, dp[0, j - 1] + (a[0] == b[j] ? 1 : 0));
			}

			for (var i = 1; i < a.Length; i++)
			{
				dp[i, 0] = Math.Min(1, dp[i - 1, 0] + (a[i] == b[0] ? 1 : 0));

				for (var j = 1; j < b.Length; j++)
				{
					dp[i, j] = a[i] == b[j]
						? (Math.Max(Math.Max(dp[i - 1, j], dp[i, j - 1]), dp[i - 1, j - 1] + 1))
						: (Math.Max(dp[i - 1, j], dp[i, j - 1]));
				}
			}

			return dp[a.Length - 1, b.Length - 1];
		}
	}
}
